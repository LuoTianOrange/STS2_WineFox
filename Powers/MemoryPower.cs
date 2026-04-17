using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Applied by Memory (回忆).
    ///     Tracks the last Skill card played each turn.
    ///     At the start of your turn, adds an Exhaust copy of the last Skill played last turn.
    ///     Amount >= 2 means upgraded: copy also gets Retain.
    /// </summary>
    [RegisterPower]
    public class MemoryPower : WineFoxPower
    {
        protected CardModel? LastSkillCard;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.MemoryPowerIcon);

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;
            if (LastSkillCard == null) return;

            Flash();
            var clone = LastSkillCard.CreateClone();
            clone.AddKeyword(CardKeyword.Exhaust);
            ConfigureClone(clone);

            var cardInstance = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, true);
            CardCmd.PreviewCardPileAdd(cardInstance);
        }

        /// <summary>Subclasses can override to add extra keywords to the generated clone.</summary>
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
