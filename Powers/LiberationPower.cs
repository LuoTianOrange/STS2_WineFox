using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Applied by Liberation (解放).
    ///     At end of turn, trigger (auto-play a dupe of) each Retained card in hand.
    ///     The original card stays in hand, keeping its Retain keyword.
    /// </summary>
    [RegisterPower]
    public class LiberationPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.LiberationPowerIcon);

        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != Owner.Side) return;

            var player = Owner.Player;
            if (player == null) return;

            var handCards = PileType.Hand.GetPile(player).Cards
                .Where(c => c.ShouldRetainThisTurn)
                .ToList();

            if (handCards.Count == 0) return;

            Flash();

            foreach (var card in handCards)
            {
                var dupe = card.CreateDupe();
                await CardCmd.AutoPlay(choiceContext, dupe, null);
            }
        }
    }
}

