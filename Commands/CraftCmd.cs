using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;
using STS2_WineFox.Hooks;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Commands
{
    public static class CraftCmd
    {
        private static readonly AttachedState<Creature, CraftTracker> Trackers = new(() => new());

        /// <summary>
        ///     Marks a card as the product of <see cref="CraftIntoHand" /> so effects like Mass Production only mirror crafted
        ///     cards.
        /// </summary>
        private static readonly AttachedState<CardModel, bool> CraftHandProductMarkers = new(() => false);

        internal static void MarkCraftHandProduct(CardModel card)
        {
            ArgumentNullException.ThrowIfNull(card);
            CraftHandProductMarkers.Set(card, true);
        }

        internal static bool IsCraftHandProduct(CardModel card)
        {
            ArgumentNullException.ThrowIfNull(card);
            return CraftHandProductMarkers.TryGetValue(card, out var v) && v;
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

        /// <summary>
        ///     以 <paramref name="crafter" /> 为主体执行合成；<paramref name="applier" /> / <paramref name="cardSource" /> 用于钩子与材料变动的来源转发（与
        ///     <c>PowerCmd</c> 一致）。<paramref name="cardSource" /> 省略时为 <c>null</c>（如能力触发的合成）。
        /// </summary>
        public static async Task<CardModel?> CraftIntoHand(
            PlayerChoiceContext choiceContext,
            Creature crafter,
            Creature? applier,
            CardModel? cardSource = null,
            CardSelectorPrefs? prefs = null)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);
            ArgumentNullException.ThrowIfNull(crafter);

            var owner = crafter.Player ??
                        throw new InvalidOperationException("Creature cannot craft without a player.");

            var combatState = crafter.CombatState ??
                              throw new InvalidOperationException("Crafter is not in combat.");

            await CraftHook.BeforeCraftIntoHand(combatState, choiceContext, crafter, applier, cardSource);

            var selectedOption = await SelectOption(choiceContext, owner, prefs);
            if (selectedOption == null)
                return null;

            if (!await TryConsumeMaterials(crafter, selectedOption.Recipe, applier, cardSource))
            {
                selectedOption.Card.RemoveFromState();
                return null;
            }

            MarkCraftHandProduct(selectedOption.Card);

            await CraftHook.BeforeCraftProductAddToCombat(combatState, crafter, selectedOption.Card);

            await CardPileCmd.AddGeneratedCardToCombat(selectedOption.Card, PileType.Hand, true);

            await CraftHook.AfterCraftProductAddToCombat(combatState, crafter, selectedOption.Card);

            return selectedOption.Card;
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
            switch (evt.Kind)
            {
                case MaterialChangeKind.Consume:
                    tracker.MaterialConsumesThisTurn++;
                    tracker.MaterialConsumesThisCombat++;
                    break;
                case MaterialChangeKind.Gain:
                    tracker.MaterialGainsThisTurn++;
                    tracker.MaterialGainsThisCombat++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(evt), evt.Kind, null);
            }
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
            }
        }
    }
}
