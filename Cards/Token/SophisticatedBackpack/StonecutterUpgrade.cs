using STS2_WineFox.Relics.Backpack.Effects;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    public class StonecutterUpgrade()
        : BackpackUpgradeCardBase<StonecutterBackpackEffect>(Const.Paths.CardStonecutterUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[StonecutterBackpackEffect.IntervalVar].BaseValue += 3m;
        }
    }
}
