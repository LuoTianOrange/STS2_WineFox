using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Cards.Token;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class DiamondSwordPower : WineFoxPower
    {
        // private bool _isEchoing;
        private bool _diamondSwordPlayedThisTurn;
        private int _usedThisTurn;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.DiamondSwordPowerIcon);

        protected override Task OnAfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return Task.CompletedTask;
            _usedThisTurn = 0;
            _diamondSwordPlayedThisTurn = false;
            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayed(
            PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner?.Creature != Owner) return;
            if (cardPlay.Card.Type != CardType.Attack) return;
            if (WineFoxActions.IsSwordEchoing) return;
            if (_usedThisTurn >= Amount) return;

            if (cardPlay.Card is DiamondSword)
                if (!_diamondSwordPlayedThisTurn)
                {
                    _diamondSwordPlayedThisTurn = true;
                    return;
                }

            _usedThisTurn++;
            WineFoxActions.IsSwordEchoing = true;
            Flash();
            await CardCmd.AutoPlay(choiceContext, cardPlay.Card, cardPlay.Target);
            WineFoxActions.IsSwordEchoing = false;
        }
    }
}
