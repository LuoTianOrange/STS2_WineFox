using MegaCrit.Sts2.Core.Localization;
using STS2_WineFox.Relics.Backpack.Effects;

namespace STS2_WineFox.Relics
{
    public static class SophisticatedBackpackEffects
    {
        public const string DescriptionVar = "UpgradeDescription";

        public static readonly IReadOnlyList<ISophisticatedBackpackEffect> All =
        [
            new BaseDrawBackpackEffect(),
            new RestockBackpackEffect(),
            new FeedingBackpackEffect(),
            new SmeltingBackpackEffect(),
            new CraftingBackpackEffect(),
            new StonecutterBackpackEffect(),
            new SavingsBackpackEffect(),
        ];

        public static IReadOnlyDictionary<Type, ISophisticatedBackpackEffect> ByType { get; } =
            All.ToDictionary(e => e.GetType(), e => e);

        public static string EnabledVar(Type effectType)
        {
            return $"EffectEnabled_{effectType.FullName}";
        }

        public static string BuildDescription(SophisticatedBackpack backpack)
        {
            var lines = new List<string>();

            var totalCombatStartDraw = All
                .Where(backpack.HasEffect)
                .OfType<ICombatStartDrawBackpackEffect>()
                .Sum(effect => effect.GetCombatStartDrawAmount(backpack));
            if (totalCombatStartDraw > 0m)
            {
                var drawLine = new LocString("relics", "STS2_WINE_FOX_RELIC_SOPHISTICATED_BACKPACK_EFFECT_COMBAT_DRAW");
                drawLine.Add("CombatStartDrawAmount", totalCombatStartDraw);
                lines.Add(drawLine.GetFormattedText());
            }

            lines.AddRange(All
                .Where(backpack.HasEffect)
                .Where(entry => entry is not ICombatStartDrawBackpackEffect)
                .Select(entry => entry.BuildDescription(backpack))
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList());
            return string.Join("\n", lines);
        }
    }
}
