using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    public class RestockUpgrade() : WineFoxCard(
        0, CardType.Power, CardRarity.Token, TargetType.None)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRestockUpgrade);

        protected override Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var backpack = Owner.Relics.OfType<Relics.SophisticatedBackpack>().FirstOrDefault();
            backpack?.ApplyRestockUpgrade();
            return Task.CompletedTask;
        }

        protected override void OnUpgrade() { }
    }
}
