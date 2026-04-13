using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace STS2_WineFox.Relics.Backpack.Effects
{
    public sealed class RestockBackpackEffect()
        : SophisticatedBackpackEffectBase(false)
            , ICombatStartDrawBackpackEffect
    {
        public const string AmountVar = "RestockDrawAmount";

        public decimal GetCombatStartDrawAmount(SophisticatedBackpack backpack)
        {
            return backpack.DynamicVars[AmountVar].BaseValue;
        }

        public override decimal ModifyHandDraw(SophisticatedBackpack backpack, Player player, decimal count)
        {
            return WithFirstRoundOwnerHandDrawBonus(backpack, player, count, GetCombatStartDrawAmount(backpack));
        }

        public override IEnumerable<DynamicVar> CreateCanonicalVars()
        {
            return [new(AmountVar, 0m)];
        }

        public override string BuildDescription(SophisticatedBackpack backpack)
        {
            return string.Empty;
        }
    }
}
