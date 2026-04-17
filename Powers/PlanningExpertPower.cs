using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Applied by PlanningExpert (规划专家).
    ///     Whenever you play a Skill card, it gains Retain.
    /// </summary>
    [RegisterPower]
    public class PlanningExpertPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.PlanningExpertPowerIcon);

        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card == null) return Task.CompletedTask;
            if (cardPlay.Card.Owner?.Creature != Owner) return Task.CompletedTask;
            if (cardPlay.Card.Type != CardType.Skill) return Task.CompletedTask;

            Flash();
            cardPlay.Card.AddKeyword(CardKeyword.Retain);
            return Task.CompletedTask;
        }
    }
}


