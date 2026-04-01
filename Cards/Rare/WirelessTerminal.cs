using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;
using STS2_WineFox.Commands;

namespace STS2_WineFox.Cards.Rare
{
    public class WirelessTerminal() : WineFoxCard(
        1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Craft];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardWirelessTerminal);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CraftCmd.CraftIntoHand(choiceContext, this);

            await CardPileCmd.Add(this, PileType.Hand);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}