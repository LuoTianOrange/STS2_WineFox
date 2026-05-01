using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using STS2_WineFox.Commands;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards
{
    public abstract class WineFoxCard(
        int baseCost,
        CardType type,
        CardRarity rarity,
        TargetType target,
        bool showInCardLibrary = true) : ModCardTemplate(baseCost, type, rarity, target, showInCardLibrary)
    {
        public sealed override Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
        {
            if (Owner == player)
                CraftCmd.ObserveTurnStarted(choiceContext, player);

            return Task.CompletedTask;
        }

        protected override void AddExtraArgsToDescription(LocString description)
        {
            base.AddExtraArgsToDescription(description);
            var suffix = "";
            if (Pile == null
                && CraftRecipeRegistry.TryGetRecipe(GetType(), out var recipe)
                && recipe.Costs.Length > 0)
                suffix = CraftRecipeFormatting.ToPreviewSuffix(recipe);

            description.Add("CraftRecipeSuffix", suffix);
        }

        protected static CardAssetProfile Art(string portraitPath)
        {
            return new(portraitPath, portraitPath);
        }
    }
}
