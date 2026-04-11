using System.Collections.ObjectModel;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox;
using STS2_WineFox.Cards;
using STS2_WineFox.Hooks;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Commands
{
    public static class CraftCmd
    {
        private static readonly AttachedState<Creature, CraftTracker> Trackers = new(() => new());
        private static readonly AttachedState<CardModel, CraftProductInfo?> CraftProductMarkers = new(() => null);

        internal static void MarkCraftProduct(CardModel card, CraftDeliveryMode deliveryMode)
        {
            ArgumentNullException.ThrowIfNull(card);
            CraftProductMarkers.Set(card, new CraftProductInfo(deliveryMode));
        }

        public static bool IsCraftProduct(CardModel card)
        {
            ArgumentNullException.ThrowIfNull(card);
            return CraftProductMarkers.TryGetValue(card, out var info) && info != null;
        }

        internal static bool IsCraftHandProduct(CardModel card)
        {
            return TryGetCraftDeliveryMode(card, out var deliveryMode) && deliveryMode == CraftDeliveryMode.ToHand;
        }

        public static bool TryGetCraftDeliveryMode(CardModel card, out CraftDeliveryMode deliveryMode)
        {
            ArgumentNullException.ThrowIfNull(card);

            if (CraftProductMarkers.TryGetValue(card, out var info) && info != null)
            {
                deliveryMode = info.DeliveryMode;
                return true;
            }

            deliveryMode = default;
            return false;
        }

        public static bool CanCraftAny(Creature creature)
        {
            return CraftRecipeRegistry.All.Any(recipe => recipe.CanCraft(creature));
        }

        public static IReadOnlyList<CraftOption> GetOptions(CombatState state, Player owner)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(owner);

            return CraftRecipeRegistry.All
                .Where(recipe => recipe.CanCraft(owner.Creature))
                .Select(recipe => new CraftOption(recipe, recipe.Factory(state, owner)))
                .ToList();
        }

        public static CardSelectorPrefs CreateSelectionPrefs()
        {
            return new(new("cards", "STS2_WINE_FOX_CHOOSE_CRAFT"), 1);
        }

        public static Task<CardModel?> Craft(
            PlayerChoiceContext choiceContext,
            Creature crafter,
            Creature? applier,
            CardModel? cardSource = null,
            CardSelectorPrefs? prefs = null,
            CraftDeliveryMode? deliveryModeOverride = null,
            Creature? autoPlayTarget = null)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);
            ArgumentNullException.ThrowIfNull(crafter);

            return CraftInternal(choiceContext, crafter, applier, cardSource, prefs, deliveryModeOverride, autoPlayTarget);
        }

        /// <summary>
        ///     以 <paramref name="crafter" /> 为主体执行合成；<paramref name="applier" /> / <paramref name="cardSource" /> 用于钩子与材料变动的来源转发（与
        ///     <c>PowerCmd</c> 一致）。<paramref name="cardSource" /> 省略时为 <c>null</c>（如能力触发的合成）。
        /// </summary>
        public static Task<CardModel?> CraftIntoHand(
            PlayerChoiceContext choiceContext,
            Creature crafter,
            Creature? applier,
            CardModel? cardSource = null,
            CardSelectorPrefs? prefs = null)
        {
            return Craft(choiceContext, crafter, applier, cardSource, prefs, CraftDeliveryMode.ToHand);
        }

        /// <summary>由打出卡牌触发合成时调用；转发为以 <c>cardSource.Owner.Creature</c> 为主体，<c>applier</c> 与 <c>cardSource</c> 与卡牌打出一致。</summary>
        public static Task<CardModel?> CraftIntoHand(PlayerChoiceContext choiceContext, CardModel cardSource,
            CardSelectorPrefs? prefs = null)
        {
            ArgumentNullException.ThrowIfNull(cardSource);

            var owner = cardSource.Owner ??
                        throw new InvalidOperationException("Craft card has no owning player.");

            var crafter = owner.Creature;
            return CraftIntoHand(choiceContext, crafter, crafter, cardSource, prefs);
        }

        public static async Task<CraftOption?> SelectOption(PlayerChoiceContext choiceContext, Player owner,
            CardSelectorPrefs? prefs = null)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);
            ArgumentNullException.ThrowIfNull(owner);

            if (owner.Creature.CombatState is not { } combatState)
                return null;

            var options = GetOptions(combatState, owner);
            if (options.Count == 0)
                return null;

            var selectedCard = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                options.Select(option => option.Card).ToList(),
                owner,
                prefs ?? CreateSelectionPrefs())).FirstOrDefault();

            var selectedOption = options.FirstOrDefault(option => ReferenceEquals(option.Card, selectedCard));
            CleanupUnselectedOptions(options, selectedOption);
            return selectedOption;
        }

        public static void ObserveTurnStarted(PlayerChoiceContext choiceContext, Player player)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);
            ArgumentNullException.ThrowIfNull(player);

            var tracker = GetTracker(player.Creature);
            if (ReferenceEquals(tracker.TurnToken, choiceContext))
                return;

            tracker.TurnToken = choiceContext;
            tracker.CraftsThisTurn = 0;
            tracker.MaterialConsumesThisTurn = 0;
            tracker.MaterialGainsThisTurn = 0;
            tracker.MaterialConsumedAmountThisTurn = 0m;
            tracker.MaterialGainedAmountThisTurn = 0m;
            tracker.MaterialConsumedByTypeThisTurn.Clear();
            tracker.MaterialGainedByTypeThisTurn.Clear();
        }

        public static void RecordCraft(Creature creature)
        {
            ArgumentNullException.ThrowIfNull(creature);

            var tracker = GetTracker(creature);
            tracker.CraftsThisTurn++;
            tracker.CraftsThisCombat++;
        }

        /// <summary>
        ///     由 <see cref="MaterialEventFlow.DispatchAfterResolved" /> 在每次材料结算后调用；勿在业务代码中重复调用。
        /// </summary>
        internal static void RecordMaterialResolved(MaterialResolvedEvent evt)
        {
            ArgumentNullException.ThrowIfNull(evt);

            var tracker = GetTracker(evt.Creature);
            var amount = evt.TotalAmount;
            switch (evt.Kind)
            {
                case MaterialChangeKind.Consume:
                    tracker.MaterialConsumesThisTurn++;
                    tracker.MaterialConsumesThisCombat++;
                    tracker.MaterialConsumedAmountThisTurn += amount;
                    tracker.MaterialConsumedAmountThisCombat += amount;
                    AddByType(tracker.MaterialConsumedByTypeThisTurn, tracker.MaterialConsumedByTypeThisCombat,
                        evt.Deltas);
                    break;
                case MaterialChangeKind.Gain:
                    tracker.MaterialGainsThisTurn++;
                    tracker.MaterialGainsThisCombat++;
                    tracker.MaterialGainedAmountThisTurn += amount;
                    tracker.MaterialGainedAmountThisCombat += amount;
                    AddByType(tracker.MaterialGainedByTypeThisTurn, tracker.MaterialGainedByTypeThisCombat,
                        evt.Deltas);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(evt), evt.Kind, null);
            }
        }

        private static async Task<CardModel?> CraftInternal(
            PlayerChoiceContext choiceContext,
            Creature crafter,
            Creature? applier,
            CardModel? cardSource,
            CardSelectorPrefs? prefs,
            CraftDeliveryMode? deliveryModeOverride,
            Creature? autoPlayTarget)
        {
            var owner = crafter.Player ??
                        throw new InvalidOperationException("Creature cannot craft without a player.");

            var combatState = crafter.CombatState ??
                              throw new InvalidOperationException("Crafter is not in combat.");

            var selectedOption = await SelectOption(choiceContext, owner, prefs);
            if (selectedOption == null)
                return null;

            if (!await TryConsumeMaterials(crafter, selectedOption.Recipe, applier, cardSource))
            {
                selectedOption.Card.RemoveFromState();
                return null;
            }

            var deliveryMode = deliveryModeOverride ?? selectedOption.Recipe.DeliveryMode;
            var craftContext = new CraftExecutionContext
            {
                ChoiceContext = choiceContext,
                Crafter = crafter,
                Applier = applier,
                SourceCard = cardSource,
                Recipe = selectedOption.Recipe,
                Product = selectedOption.Card,
                DeliveryMode = deliveryMode,
                AutoPlayTarget = autoPlayTarget,
            };

            await CraftHook.BeforeCraft(combatState, craftContext);
            await DeliverCraftProduct(combatState, craftContext);
            return selectedOption.Card;
        }

        private static async Task DeliverCraftProduct(CombatState combatState, CraftExecutionContext context)
        {
            var deliveryContext = context.Product is ICraftChoiceEffect
                ? context with { DeliveryMode = CraftDeliveryMode.ImmediateEffect }
                : context;

            MarkCraftProduct(deliveryContext.Product, deliveryContext.DeliveryMode);
            await CraftHook.BeforeCraftProductDelivered(combatState, deliveryContext);

            switch (deliveryContext.DeliveryMode)
            {
                case CraftDeliveryMode.ToHand:
                    await CardPileCmd.AddGeneratedCardToCombat(deliveryContext.Product, PileType.Hand, true);
                    break;
                case CraftDeliveryMode.AutoPlay:
                    await CardCmd.AutoPlay(deliveryContext.ChoiceContext, deliveryContext.Product,
                        deliveryContext.AutoPlayTarget);
                    break;
                case CraftDeliveryMode.ImmediateEffect:
                    if (deliveryContext.Product is not ICraftChoiceEffect choiceEffect)
                        throw new InvalidOperationException(
                            $"Craft product {deliveryContext.Product.GetType().Name} does not implement {nameof(ICraftChoiceEffect)}.");

                    await choiceEffect.OnCraftChosen(deliveryContext);
                    if (deliveryContext.Product.CombatState != null)
                        deliveryContext.Product.RemoveFromState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await CraftHook.AfterCraftProductDelivered(combatState, deliveryContext);
        }

        private static void AddByType(
            Dictionary<Type, decimal> turnByType,
            Dictionary<Type, decimal> combatByType,
            IReadOnlyList<MaterialDelta> deltas)
        {
            foreach (var d in deltas)
            {
                if (d.Amount == 0m)
                    continue;

                turnByType[d.MaterialType] = turnByType.GetValueOrDefault(d.MaterialType) + d.Amount;
                combatByType[d.MaterialType] = combatByType.GetValueOrDefault(d.MaterialType) + d.Amount;
            }
        }

        /// <summary>内部字典的快照，调用方无法修改 <see cref="CraftTracker" /> 中的累计。</summary>
        private static IReadOnlyDictionary<Type, decimal> SnapshotMaterialByType(Dictionary<Type, decimal> source)
        {
            return new ReadOnlyDictionary<Type, decimal>(new Dictionary<Type, decimal>(source));
        }

        public static bool HasCraftedThisTurn(Creature creature)
        {
            return GetCraftCountThisTurn(creature) > 0;
        }

        public static bool HasCraftedThisTurn(Player player)
        {
            return HasCraftedThisTurn(player.Creature);
        }

        public static int GetCraftCountThisTurn(Creature creature)
        {
            return GetTracker(creature).CraftsThisTurn;
        }

        public static int GetCraftCountThisTurn(Player player)
        {
            return GetCraftCountThisTurn(player.Creature);
        }

        public static int GetCraftCountThisCombat(Creature creature)
        {
            return GetTracker(creature).CraftsThisCombat;
        }

        public static int GetCraftCountThisCombat(Player player)
        {
            return GetCraftCountThisCombat(player.Creature);
        }

        public static bool HasConsumedMaterialThisTurn(Creature creature)
        {
            return GetMaterialConsumeCountThisTurn(creature) > 0;
        }

        public static bool HasConsumedMaterialThisTurn(Player player)
        {
            return HasConsumedMaterialThisTurn(player.Creature);
        }

        public static int GetMaterialConsumeCountThisTurn(Creature creature)
        {
            return GetTracker(creature).MaterialConsumesThisTurn;
        }

        public static int GetMaterialConsumeCountThisTurn(Player player)
        {
            return GetMaterialConsumeCountThisTurn(player.Creature);
        }

        public static int GetMaterialConsumeCountThisCombat(Creature creature)
        {
            return GetTracker(creature).MaterialConsumesThisCombat;
        }

        public static int GetMaterialConsumeCountThisCombat(Player player)
        {
            return GetMaterialConsumeCountThisCombat(player.Creature);
        }

        public static decimal GetMaterialConsumedAmountThisTurn(Creature creature)
        {
            return GetTracker(creature).MaterialConsumedAmountThisTurn;
        }

        public static decimal GetMaterialConsumedAmountThisTurn(Player player)
        {
            return GetMaterialConsumedAmountThisTurn(player.Creature);
        }

        public static decimal GetMaterialConsumedAmountThisCombat(Creature creature)
        {
            return GetTracker(creature).MaterialConsumedAmountThisCombat;
        }

        public static decimal GetMaterialConsumedAmountThisCombat(Player player)
        {
            return GetMaterialConsumedAmountThisCombat(player.Creature);
        }

        public static bool HasGainedMaterialThisTurn(Creature creature)
        {
            return GetMaterialGainCountThisTurn(creature) > 0;
        }

        public static bool HasGainedMaterialThisTurn(Player player)
        {
            return HasGainedMaterialThisTurn(player.Creature);
        }

        public static int GetMaterialGainCountThisTurn(Creature creature)
        {
            return GetTracker(creature).MaterialGainsThisTurn;
        }

        public static int GetMaterialGainCountThisTurn(Player player)
        {
            return GetMaterialGainCountThisTurn(player.Creature);
        }

        public static int GetMaterialGainCountThisCombat(Creature creature)
        {
            return GetTracker(creature).MaterialGainsThisCombat;
        }

        public static int GetMaterialGainCountThisCombat(Player player)
        {
            return GetMaterialGainCountThisCombat(player.Creature);
        }

        public static decimal GetMaterialGainedAmountThisTurn(Creature creature)
        {
            return GetTracker(creature).MaterialGainedAmountThisTurn;
        }

        public static decimal GetMaterialGainedAmountThisTurn(Player player)
        {
            return GetMaterialGainedAmountThisTurn(player.Creature);
        }

        public static decimal GetMaterialGainedAmountThisCombat(Creature creature)
        {
            return GetTracker(creature).MaterialGainedAmountThisCombat;
        }

        public static decimal GetMaterialGainedAmountThisCombat(Player player)
        {
            return GetMaterialGainedAmountThisCombat(player.Creature);
        }

        /// <summary>
        ///     按材料 Power 类型分别累计的消耗量（本回合），只读快照。
        /// </summary>
        public static IReadOnlyDictionary<Type, decimal> GetMaterialConsumedAmountsByTypeThisTurn(Creature creature)
        {
            return SnapshotMaterialByType(GetTracker(creature).MaterialConsumedByTypeThisTurn);
        }

        public static IReadOnlyDictionary<Type, decimal> GetMaterialConsumedAmountsByTypeThisTurn(Player player)
        {
            return GetMaterialConsumedAmountsByTypeThisTurn(player.Creature);
        }

        /// <summary>
        ///     按材料 Power 类型分别累计的消耗量（本场战斗），只读快照。
        /// </summary>
        public static IReadOnlyDictionary<Type, decimal> GetMaterialConsumedAmountsByTypeThisCombat(Creature creature)
        {
            return SnapshotMaterialByType(GetTracker(creature).MaterialConsumedByTypeThisCombat);
        }

        public static IReadOnlyDictionary<Type, decimal> GetMaterialConsumedAmountsByTypeThisCombat(Player player)
        {
            return GetMaterialConsumedAmountsByTypeThisCombat(player.Creature);
        }

        /// <summary>
        ///     按材料 Power 类型分别累计的获得量（本回合），只读快照。
        /// </summary>
        public static IReadOnlyDictionary<Type, decimal> GetMaterialGainedAmountsByTypeThisTurn(Creature creature)
        {
            return SnapshotMaterialByType(GetTracker(creature).MaterialGainedByTypeThisTurn);
        }

        public static IReadOnlyDictionary<Type, decimal> GetMaterialGainedAmountsByTypeThisTurn(Player player)
        {
            return GetMaterialGainedAmountsByTypeThisTurn(player.Creature);
        }

        /// <summary>
        ///     按材料 Power 类型分别累计的获得量（本场战斗），只读快照。
        /// </summary>
        public static IReadOnlyDictionary<Type, decimal> GetMaterialGainedAmountsByTypeThisCombat(Creature creature)
        {
            return SnapshotMaterialByType(GetTracker(creature).MaterialGainedByTypeThisCombat);
        }

        public static IReadOnlyDictionary<Type, decimal> GetMaterialGainedAmountsByTypeThisCombat(Player player)
        {
            return GetMaterialGainedAmountsByTypeThisCombat(player.Creature);
        }

        /// <summary>
        ///     消耗 <paramref name="crafter" /> 身上的配方材料；来源参数转发至 <c>PowerCmd.ModifyAmount</c>。<paramref name="cardSource" />
        ///     省略时为 <c>null</c>。
        /// </summary>
        public static async Task<bool> TryConsumeMaterials(
            Creature crafter,
            CraftRecipe recipe,
            Creature? applier,
            CardModel? cardSource = null)
        {
            ArgumentNullException.ThrowIfNull(crafter);
            ArgumentNullException.ThrowIfNull(recipe);

            if (!recipe.CanCraft(crafter))
                return false;

            var powersToConsume = new List<(PowerModel Power, decimal Amount)>();
            foreach (var cost in recipe.Costs)
            {
                var power = crafter.Powers.FirstOrDefault(p => p.GetType() == cost.PowerType);
                if (power == null || power.Amount < cost.Amount)
                    return false;

                powersToConsume.Add((power, cost.Amount));
            }

            var deltas = powersToConsume
                .Select(entry => new MaterialDelta(entry.Power.GetType(), entry.Amount))
                .ToList();
            var totalAmount = deltas.Sum(d => d.Amount);
            var consumeEvent = new MaterialConsumeEvent
            {
                Creature = crafter,
                SourceCard = cardSource,
                Deltas = deltas,
                TotalAmount = totalAmount,
            };

            await MaterialEventFlow.DispatchBeforeConsume(consumeEvent);
            foreach (var (power, amount) in powersToConsume)
                await PowerCmd.ModifyAmount(power, -amount, applier, cardSource);
            await MaterialEventFlow.DispatchAfterConsume(consumeEvent);
            await MaterialEventFlow.DispatchAfterResolved(new()
            {
                Creature = crafter,
                SourceCard = cardSource,
                Deltas = deltas,
                TotalAmount = totalAmount,
                Kind = MaterialChangeKind.Consume,
                AppliedStressMultiplier = false,
            });

            RecordCraft(crafter);
            return true;
        }

        private static CraftTracker GetTracker(Creature creature)
        {
            ArgumentNullException.ThrowIfNull(creature);

            var tracker = Trackers.GetOrCreate(creature);
            if (!ReferenceEquals(tracker.CombatState, creature.CombatState))
                tracker.ResetForCombat(creature.CombatState);

            return tracker;
        }

        private static void CleanupUnselectedOptions(IEnumerable<CraftOption> options, CraftOption? selectedOption)
        {
            foreach (var option in options)
            {
                if (ReferenceEquals(option, selectedOption))
                    continue;

                option.Card.RemoveFromState();
            }
        }

        private sealed record CraftProductInfo(CraftDeliveryMode DeliveryMode);

        private sealed class CraftTracker
        {
            public CombatState? CombatState { get; private set; }
            public object? TurnToken { get; set; }
            public int CraftsThisTurn { get; set; }
            public int CraftsThisCombat { get; set; }
            public int MaterialConsumesThisTurn { get; set; }
            public int MaterialConsumesThisCombat { get; set; }
            public int MaterialGainsThisTurn { get; set; }
            public int MaterialGainsThisCombat { get; set; }
            public decimal MaterialConsumedAmountThisTurn { get; set; }
            public decimal MaterialConsumedAmountThisCombat { get; set; }
            public decimal MaterialGainedAmountThisTurn { get; set; }
            public decimal MaterialGainedAmountThisCombat { get; set; }

            public Dictionary<Type, decimal> MaterialConsumedByTypeThisTurn { get; } = new();
            public Dictionary<Type, decimal> MaterialConsumedByTypeThisCombat { get; } = new();
            public Dictionary<Type, decimal> MaterialGainedByTypeThisTurn { get; } = new();
            public Dictionary<Type, decimal> MaterialGainedByTypeThisCombat { get; } = new();

            public void ResetForCombat(CombatState? combatState)
            {
                CombatState = combatState;
                TurnToken = null;
                CraftsThisTurn = 0;
                CraftsThisCombat = 0;
                MaterialConsumesThisTurn = 0;
                MaterialConsumesThisCombat = 0;
                MaterialGainsThisTurn = 0;
                MaterialGainsThisCombat = 0;
                MaterialConsumedAmountThisTurn = 0m;
                MaterialConsumedAmountThisCombat = 0m;
                MaterialGainedAmountThisTurn = 0m;
                MaterialGainedAmountThisCombat = 0m;
                MaterialConsumedByTypeThisTurn.Clear();
                MaterialConsumedByTypeThisCombat.Clear();
                MaterialGainedByTypeThisTurn.Clear();
                MaterialGainedByTypeThisCombat.Clear();
            }
        }
    }
}
