using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Commands;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    public class WirelessTerminal() : WineFoxCard(
        1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Craft];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardWirelessTerminal);

        public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
            CardModel card,
            bool isAutoPlay,
            ResourceInfo resources,
            PileType pileType,
            CardPilePosition position)
        {
            if (!ReferenceEquals(card, this))
                return (pileType, position);

            if (pileType == PileType.Discard)
                return (PileType.Hand, CardPilePosition.Top);

            return (pileType, position);
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CraftCmd.CraftIntoHand(choiceContext, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}
