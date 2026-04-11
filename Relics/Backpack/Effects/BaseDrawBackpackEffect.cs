using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace STS2_WineFox.Relics.Backpack.Effects
{
    public sealed class BaseDrawBackpackEffect()
        : SophisticatedBackpackEffectBase(true)
            , ICombatStartDrawBackpackEffect
    {
        public const string AmountVar = "BaseDrawAmount";

        public decimal GetCombatStartDrawAmount(SophisticatedBackpack backpack)
        {
            return backpack.DynamicVars[AmountVar].BaseValue;
        }

        public override IEnumerable<DynamicVar> CreateCanonicalVars()
        {
            return [new(AmountVar, 1m)];
        }

        public override string BuildDescription(SophisticatedBackpack backpack)
        {
            return string.Empty;
        }
    }
}
