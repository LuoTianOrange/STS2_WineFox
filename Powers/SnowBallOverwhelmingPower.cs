using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class SnowBallOverwhelmingPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.SnowBallOverwhelmingPowerIcon);

        public override bool TryModifyEnergyCostInCombat(
            CardModel card,
            decimal originalCost,
            out decimal modifiedCost)
        {
            modifiedCost = originalCost;

            if (card.Owner.Creature != Owner || card.Type != CardType.Skill)
                return false;

            var pile = card.Pile?.Type;
            if (pile != PileType.Hand && pile != PileType.Play)
                return false;

            modifiedCost = 0m;
            return true;
        }

        public override async Task BeforeCardPlayed(CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Skill)
                return;

            var pile = cardPlay.Card.Pile?.Type;
            if (pile != PileType.Hand && pile != PileType.Play)
                return;

            await PowerCmd.Decrement(this);
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == Owner.Side)
                await PowerCmd.Remove(this);
        }
    }
}
