using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class ShardCopyPower : WineFoxPower
    {
        private CardModel? _initialSourceCard;
        private bool _skipInitialSourceCardPlay;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ShardCopyPowerIcon);

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            _initialSourceCard = cardSource;
            _skipInitialSourceCardPlay = cardSource != null;
            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayedLate(PlayerChoiceContext context, CardPlay cardPlay)
        {
            var card = cardPlay.Card;

            if (_skipInitialSourceCardPlay && ReferenceEquals(card, _initialSourceCard))
            {
                _skipInitialSourceCardPlay = false;
                return;
            }

            if (card.Owner.Creature != Owner) return;
            if (!card.IsMagic()) return;
            if (Amount <= 0m) return;

            Flash();
            await PowerCmd.Apply<ChantPower>(Owner, Amount, Owner, card);
        }
    }
}
