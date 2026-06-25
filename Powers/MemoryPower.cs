using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Applied by Memory (回忆).
    ///     Tracks the last Skill card played each turn.
    ///     At the start of your turn, adds an Exhaust copy of the last Skill played last turn.
    /// </summary>
    [RegisterPower]
    public class MemoryPower : WineFoxPower
    {
        private CardModel? LastSkillCard;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.MemoryPowerIcon);

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;
            if (LastSkillCard == null) return;

            Flash();
            var copyCount = Math.Max(0, (int)Amount);
            for (var i = 0; i < copyCount; i++)
            {
                var clone = LastSkillCard.CreateClone();
                clone.AddKeyword(CardKeyword.Exhaust);
                ConfigureClone(clone);

                var cardInstance = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, player);
                CardCmd.PreviewCardPileAdd(cardInstance);
            }
        }

        protected virtual void ConfigureClone(CardModel clone) { }

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner.Creature != Owner) return Task.CompletedTask;
            if (cardPlay.Card.Type != CardType.Skill) return Task.CompletedTask;

            LastSkillCard = cardPlay.Card;
            return Task.CompletedTask;
        }
    }
}
