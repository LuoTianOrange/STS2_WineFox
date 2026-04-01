using System.Globalization;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Cards
{
    public static class CraftRecipeFormatting
    {
        /// <summary>
        ///     Localized parenthetical block for craft preview (leading newline + parentheses).
        /// </summary>
        public static string ToPreviewSuffix(CraftRecipe recipe)
        {
            if (recipe.Costs.Length == 0)
                return "";

            var join = new LocString("cards", "STS2_WINE_FOX_CRAFT_RECIPE_JOIN").GetFormattedText();
            var parts = new string[recipe.Costs.Length];
            for (var i = 0; i < recipe.Costs.Length; i++)
            {
                var cost = recipe.Costs[i];
                var segment = new LocString("cards", "STS2_WINE_FOX_CRAFT_RECIPE_SEGMENT");
                segment.Add("Amount", FormatAmount(cost.Amount));
                segment.Add("Material", MaterialTitle(cost.PowerType));
                parts[i] = segment.GetFormattedText();
            }

            return "\n(" + string.Join(join, parts) + ")";
        }

        private static string MaterialTitle(Type powerType)
        {
            var entry = ModelDb.GetId(powerType).Entry;
            return new LocString("powers", entry + ".title").GetFormattedText();
        }

        private static string FormatAmount(decimal amount)
        {
            return amount == decimal.Truncate(amount)
                ? decimal.ToInt32(amount).ToString(CultureInfo.InvariantCulture)
                : amount.ToString(CultureInfo.InvariantCulture);
        }
    }
}
