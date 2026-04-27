using STS2_WineFox.Character;
using STS2_WineFox.Relics.Backpack.Effects;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    [RegisterCard(typeof(WineFoxCraftingCardPool))]
    public class SmeltingUpgrade() : BackpackUpgradeCardBase<SmeltingBackpackEffect>(Const.Paths.CardSmeltingUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[SmeltingBackpackEffect.IronVar].BaseValue += 1m;
        }
    }
}
