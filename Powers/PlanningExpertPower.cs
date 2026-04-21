using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
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

        protected override object? InitInternalData() => new List<CardModel>();

        private List<CardModel> PendingDiscardSkills => GetInternalData<List<CardModel>>();

        public override Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
        {
            var pending = PendingDiscardSkills;
            pending.Clear();

            if (player != Owner.Player || !Hook.ShouldFlush(player.Creature.CombatState, player))
                return Task.CompletedTask;

            var hand = PileType.Hand.GetPile(player);
            foreach (var card in hand.Cards)
            {
                if (card.Type != CardType.Skill || card.ShouldRetainThisTurn)
                    continue;

                pending.Add(card);
            }

            return Task.CompletedTask;
        }

        public override Task AfterTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != CombatSide.Player)
                return Task.CompletedTask;

            var player = Owner.Player;
            if (player == null)
                return Task.CompletedTask;

            var pending = PendingDiscardSkills;
            if (pending.Count == 0)
                return Task.CompletedTask;

            var any = false;
            foreach (var card in pending)
            {
                if (!ReferenceEquals(card.Owner, player))
                    continue;
                if (card.HasBeenRemovedFromState)
                    continue;
                if (card.Type != CardType.Skill)
                    continue;
                if (card.Keywords.Contains(CardKeyword.Retain))
                    continue;

                card.AddKeyword(CardKeyword.Retain);
                any = true;
            }

            pending.Clear();
            if (any)
                Flash();

            return Task.CompletedTask;
        }
    }
}
