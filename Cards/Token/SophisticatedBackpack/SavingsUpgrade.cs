using STS2_WineFox.Character;
using STS2_WineFox.Relics.Backpack.Effects;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class SavingsUpgrade() : BackpackUpgradeCardBase<SavingsBackpackEffect>(Const.Paths.CardSavingsUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[SavingsBackpackEffect.GoldVar].BaseValue += 15m;
        }
    }
}
