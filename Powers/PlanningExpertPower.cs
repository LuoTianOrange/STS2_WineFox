using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class PlanningExpertPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.PlanningExpertPowerIcon);

        public override Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
        {
            TryGrantRetain(card);

            return Task.CompletedTask;
        }

        public override Task AfterCardChangedPilesLate(CardModel card, PileType oldPile, AbstractModel? source)
        {
            if (oldPile != PileType.Hand)
                return Task.CompletedTask;
            if (card.Pile?.Type != PileType.Discard)
                return Task.CompletedTask;

            TryGrantRetain(card);
            return Task.CompletedTask;
        }

        public override Task AfterFlush(
            PlayerChoiceContext choiceContext,
            Player player,
            IReadOnlyCollection<CardModel> flushedCards,
            IReadOnlyCollection<CardModel> retainedCards)
        {
            if (player.Creature != Owner)
                return Task.CompletedTask;

            foreach (var card in flushedCards)
            {
                if (!CanGrantRetain(card))
                    continue;

                TryGrantRetain(card);
            }

            return Task.CompletedTask;
        }

        private void TryGrantRetain(CardModel card)
        {
            if (!CanGrantRetain(card))
                return;

            card.AddKeyword(CardKeyword.Retain);
            Flash();
        }

        private bool CanGrantRetain(CardModel card)
        {
            if (card.Owner.Creature != Owner)
                return false;
            if (card.Type != CardType.Skill)
                return false;
            if (card.HasBeenRemovedFromState)
                return false;
            if (card.Keywords.Contains(CardKeyword.Retain))
                return false;

            return true;
        }
    }
}
