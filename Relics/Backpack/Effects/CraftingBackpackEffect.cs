using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using STS2_WineFox.Cards.Token.SophisticatedBackpack;
using STS2_WineFox.Commands;

namespace STS2_WineFox.Relics.Backpack.Effects
{
    public sealed class CraftingBackpackEffect()
        : SophisticatedBackpackEffectBase(false)
    {
        private const string CraftProgressCounter = "progress";

        public const string ThresholdVar = "CraftThreshold";
        public const string BonusVar = "CraftBonusAmount";

        public override IEnumerable<DynamicVar> CreateCanonicalVars()
        {
            return
            [
                new(ThresholdVar, 0m),
                new(BonusVar, 0m),
            ];
        }

        public override string BuildDescription(SophisticatedBackpack backpack)
        {
            var mainLine = BuildLine(backpack, "STS2_WINE_FOX_RELIC_SOPHISTICATED_BACKPACK_EFFECT_CRAFT", ThresholdVar,
                BonusVar);
            var threshold = Math.Max(1, backpack.DynamicVars[ThresholdVar].IntValue);
            var progress = backpack.GetEffectStateInt<CraftingBackpackEffect>(CraftProgressCounter);
            var remaining = threshold - progress;
            if (remaining <= 0)
                remaining = threshold;

            var remainingLine = new LocString("relics", "STS2_WINE_FOX_RELIC_SOPHISTICATED_BACKPACK_REMAINING_CRAFT");
            remainingLine.Add("Remaining", remaining);
            return $"{mainLine}\n{remainingLine.GetFormattedText()}";
        }

        public override async Task AfterCraftProductDelivered(SophisticatedBackpack backpack,
            CraftExecutionContext context)
        {
            if (!ReferenceEquals(context.Crafter, backpack.Owner.Creature)) return;
            if (context.IsBonusCraft) return;

            var threshold = Math.Max(1, backpack.DynamicVars[ThresholdVar].IntValue);

            if (context.Product is CraftUpgrade)
            {
                backpack.SetEffectStateInt<CraftingBackpackEffect>(CraftProgressCounter, 0);
                backpack.RefreshDescriptionText();
                return;
            }

            var progress = backpack.IncrementEffectStateInt<CraftingBackpackEffect>(CraftProgressCounter);
            if (progress < threshold)
            {
                backpack.RefreshDescriptionText();
                return;
            }

            backpack.SetEffectStateInt<CraftingBackpackEffect>(CraftProgressCounter, 0);
            var bonusCount = Math.Max(1, backpack.DynamicVars[BonusVar].IntValue);
            backpack.RefreshDescriptionText();

            backpack.NotifyBackpackEffectTriggered();
            for (var i = 0; i < bonusCount; i++)
                await CraftCmd.Craft(context.ChoiceContext, context.Crafter, context.Applier, context.SourceCard,
                    isBonusCraft: true);
        }
    }
}
