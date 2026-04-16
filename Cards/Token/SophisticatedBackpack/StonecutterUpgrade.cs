using STS2_WineFox.Character;
using STS2_WineFox.Relics.Backpack.Effects;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class StonecutterUpgrade()
        : BackpackUpgradeCardBase<StonecutterBackpackEffect>(Const.Paths.CardStonecutterUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[StonecutterBackpackEffect.IntervalVar].BaseValue += 3m;
        }
    }
}
