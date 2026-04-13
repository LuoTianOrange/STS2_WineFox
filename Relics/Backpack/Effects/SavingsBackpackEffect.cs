using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace STS2_WineFox.Relics.Backpack.Effects
{
    public sealed class SavingsBackpackEffect()
        : SophisticatedBackpackEffectBase(false)
    {
        public const string GoldVar = "SavingsGoldAmount";

        public override IEnumerable<DynamicVar> CreateCanonicalVars()
        {
            return [new(GoldVar, 0m)];
        }

        public override string BuildDescription(SophisticatedBackpack backpack)
        {
            return BuildLine(backpack, "STS2_WINE_FOX_RELIC_SOPHISTICATED_BACKPACK_EFFECT_SAVINGS", GoldVar);
        }

        public override Task AfterCombatEnd(SophisticatedBackpack backpack, CombatRoom room)
        {
            var goldAmount = Math.Max(0, backpack.DynamicVars[GoldVar].IntValue);
            if (goldAmount <= 0)
                return Task.CompletedTask;

            backpack.NotifyBackpackEffectTriggered();
            room.AddExtraReward(backpack.Owner, new GoldReward(goldAmount, backpack.Owner));
            return Task.CompletedTask;
        }
    }
}
