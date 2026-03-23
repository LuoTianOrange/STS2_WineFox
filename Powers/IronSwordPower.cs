using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Cards.Token;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class IronSwordPower : WineFoxPower
    {
        private bool _ironSwordPlayedThisTurn;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => new(
            Const.Paths.IronSwordPowerIcon,
            Const.Paths.IronSwordPowerIcon);

        public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return Task.CompletedTask;
            _ironSwordPlayedThisTurn = false;   // 每回合重置，避免跨回合误判
            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayed(
            PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner?.Creature != Owner) return;
            if (cardPlay.Card.Type != CardType.Attack) return;
            if (WineFoxActions.IsSwordEchoing) return;
            if ((decimal)Amount <= 0m) return;

            // 铁剑本回合第一次打出时不回响自身
            if (cardPlay.Card is IronSword)
            {
                if (!_ironSwordPlayedThisTurn)
                {
                    _ironSwordPlayedThisTurn = true;
                    return;
                }
            }

            WineFoxActions.IsSwordEchoing = true;
            Flash();
            await CardCmd.AutoPlay(choiceContext, cardPlay.Card, cardPlay.Target);
            WineFoxActions.IsSwordEchoing = false;

            // 消耗一次回响次数，归零后移除
            await PowerCmd.ModifyAmount(this, -1m, null, null);
            if ((decimal)Amount <= 0m)
                await PowerCmd.Remove(this);
        }
    }
}
