using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class WirelessTerminal() : WineFoxCard(
        1, CardType.Skill, CardRarity.Rare, TargetType.None), ICraftingCard
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Craft];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardWirelessTerminal,
            Const.Paths.CardWirelessTerminal,
            FrameMaterialPath: Const.Paths.CardWirelessTerminalRainbowFrameMat);

        protected override PileType GetResultPileType()
        {
            var result = base.GetResultPileType();
            return result != PileType.Discard ? result : PileType.Hand;
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
