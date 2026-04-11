using STS2_WineFox.Relics.Backpack.Effects;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    public class FeedingUpgrade() : BackpackUpgradeCardBase<FeedingBackpackEffect>(Const.Paths.CardFeedingUpgrade)
    {
        protected override void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
            backpack.DynamicVars[FeedingBackpackEffect.RegenVar].BaseValue += 1m;
        }
    }
}
