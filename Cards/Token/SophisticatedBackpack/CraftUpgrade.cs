using STS2_WineFox.Character;
using STS2_WineFox.Relics.Backpack.Effects;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class CraftUpgrade() : BackpackUpgradeCardBase<CraftingBackpackEffect>(Const.Paths.CardCraftUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[CraftingBackpackEffect.ThresholdVar].BaseValue += 2m;
            backpack.DynamicVars[CraftingBackpackEffect.BonusVar].BaseValue += 1m;
        }
    }
}
