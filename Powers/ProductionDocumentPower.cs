using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class ProductionDocumentPower : WineFoxPower
    {
        private readonly HashSet<CardModel> _trackedCards = new();
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ProductionDocumentPowerIcon);

        public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
        {
            if (card.Owner?.Creature != Owner) return Task.CompletedTask;
            if (!addedByPlayer && !card.IsClone) return Task.CompletedTask;

            card.AddKeyword(CardKeyword.Retain);
            _trackedCards.Add(card);
            return Task.CompletedTask;
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != Owner.Side) return;
            if (Owner.Player == null) return;

            var handCards = PileType.Hand.GetPile(Owner.Player).Cards;
            var retainedCount = handCards.Count(c => _trackedCards.Contains(c));

            if (retainedCount <= 0) return;

            Flash();
            await CreatureCmd.GainBlock(Owner, retainedCount * 2m, ValueProp.Unpowered, null);
        }
    }
}
