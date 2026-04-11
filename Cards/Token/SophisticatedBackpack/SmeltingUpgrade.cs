using STS2_WineFox.Relics.Backpack.Effects;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    public class SmeltingUpgrade() : BackpackUpgradeCardBase<SmeltingBackpackEffect>(Const.Paths.CardSmeltingUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[SmeltingBackpackEffect.IronVar].BaseValue += 1m;
        }
    }
}
