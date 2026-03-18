using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Cards.Basic;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class DiggingPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => new(
            Const.Paths.DiggingPowerIcon,
            Const.Paths.DiggingPowerIcon
        );

        public override async Task AfterCardPlayed(
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay)
        {
            if (cardPlay.Card.Type == CardType.Attack 
                && cardPlay.Card is not BasicMine)
            {
                await WineFoxActions.GainMaterials<WoodPower, StonePower>(cardPlay.Card, 1m, 1m);
            }
        }
    }
}