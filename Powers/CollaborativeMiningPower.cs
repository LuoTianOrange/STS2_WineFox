using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class CollaborativeMiningPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.CollaborativeMiningPowerIcon);

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.IsAutoPlay) return;
            if (cardPlay.Card.Type != CardType.Power) return;

            var cardOwner = cardPlay.Card.Owner?.Creature;
            if (cardOwner == null) return;
            if (cardOwner == Owner) return;
            if (cardOwner.Side != Owner.Side) return;

            Flash();
            await MaterialCmd.GainAllMaterials(Owner, Amount, false);
        }
    }
}
