using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Commands
{
    public static class CraftCmd
    {
        private static readonly AttachedState<Creature, CraftTracker> Trackers = new(() => new());

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

        public static async Task<CardModel?> CraftIntoHand(PlayerChoiceContext choiceContext, CardModel source,
            CardSelectorPrefs? prefs = null)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);
            ArgumentNullException.ThrowIfNull(source);

            var selectedOption = await SelectOption(choiceContext, source, prefs);
            if (selectedOption == null)
                return null;

            if (!await TryConsumeMaterials(source, selectedOption.Recipe))
            {
                selectedOption.Card.RemoveFromState();
                return null;
            }

            await CardPileCmd.AddGeneratedCardToCombat(selectedOption.Card, PileType.Hand, true);
            return selectedOption.Card;
        }

        public static async Task<CraftOption?> SelectOption(PlayerChoiceContext choiceContext, CardModel source,
            CardSelectorPrefs? prefs = null)
        {
            ArgumentNullException.ThrowIfNull(choiceContext);
            ArgumentNullException.ThrowIfNull(source);

            var owner = source.Owner;
            if (owner?.Creature.CombatState is not { } combatState)
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
        }

        public static void RecordCraft(Creature creature)
        {
            ArgumentNullException.ThrowIfNull(creature);

            var tracker = GetTracker(creature);
            tracker.CraftsThisTurn++;
            tracker.CraftsThisCombat++;
        }

        public static void RecordMaterialConsume(Creature creature)
        {
            ArgumentNullException.ThrowIfNull(creature);

            var tracker = GetTracker(creature);
            tracker.MaterialConsumesThisTurn++;
            tracker.MaterialConsumesThisCombat++;
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

        public static async Task<bool> TryConsumeMaterials(CardModel source, CraftRecipe recipe)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(recipe);

            var owner = source.Owner?.Creature ??
                        throw new InvalidOperationException("Craft source has no owner creature.");
            if (!recipe.CanCraft(owner))
                return false;

            var powersToConsume = new List<(PowerModel Power, decimal Amount)>();
            foreach (var cost in recipe.Costs)
            {
                var power = owner.Powers.FirstOrDefault(power => power.GetType() == cost.PowerType);
                if (power == null || power.Amount < cost.Amount)
                    return false;

                powersToConsume.Add((power, cost.Amount));
            }

            foreach (var (power, amount) in powersToConsume)
                await PowerCmd.ModifyAmount(power, -amount, null, source);

            RecordMaterialConsume(owner);
            RecordCraft(owner);
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

            public void ResetForCombat(CombatState? combatState)
            {
                CombatState = combatState;
                TurnToken = null;
                CraftsThisTurn = 0;
                CraftsThisCombat = 0;
                MaterialConsumesThisTurn = 0;
                MaterialConsumesThisCombat = 0;
            }
        }
    }
}
