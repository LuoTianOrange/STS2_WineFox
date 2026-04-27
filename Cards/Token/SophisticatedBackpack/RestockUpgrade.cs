using STS2_WineFox.Character;
using STS2_WineFox.Relics.Backpack.Effects;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    [RegisterCard(typeof(WineFoxCraftingCardPool))]
    public class RestockUpgrade() : BackpackUpgradeCardBase<RestockBackpackEffect>(Const.Paths.CardRestockUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[RestockBackpackEffect.AmountVar].BaseValue += 2m;
        }
    }
}
