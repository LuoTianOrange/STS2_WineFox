using STS2_WineFox.Relics.Backpack.Effects;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    public class SavingsUpgrade() : BackpackUpgradeCardBase<SavingsBackpackEffect>(Const.Paths.CardSavingsUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[SavingsBackpackEffect.GoldVar].BaseValue += 15m;
        }
    }
}
