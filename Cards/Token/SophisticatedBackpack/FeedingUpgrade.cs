using STS2_WineFox.Character;
using STS2_WineFox.Relics.Backpack.Effects;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    [RegisterCard(typeof(WineFoxCraftingCardPool))]
    public class FeedingUpgrade() : BackpackUpgradeCardBase<FeedingBackpackEffect>(Const.Paths.CardFeedingUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[FeedingBackpackEffect.RegenVar].BaseValue += 1m;
        }
    }
}
