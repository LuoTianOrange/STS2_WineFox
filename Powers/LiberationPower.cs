using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Applied by Liberation (解放).
    ///     At end of turn, trigger each card retained in hand by auto-playing a dupe.
    ///     The original card stays in hand, keeping its Retain keyword.
    /// </summary>
    [RegisterPower]
    public class LiberationPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.LiberationPowerIcon);

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != Owner.Side) return;

            var player = Owner.Player;
            if (player == null) return;

            var handCards = PileType.Hand.GetPile(player).Cards.ToList();
            if (handCards.Count == 0) return;

            var localNetId = LocalContext.NetId;
            if (!localNetId.HasValue) return;

            var combatState = player.Creature.CombatState;
            if (combatState == null) return;

            Flash();

            foreach (var card in handCards)
            {
                var hookChoiceContext = new HookPlayerChoiceContext(this, localNetId.Value, combatState, GameActionType.Combat);
                var dupe = card.CreateDupe();
                var autoPlayTask = CardCmd.AutoPlay(hookChoiceContext, dupe, null);
                if (!(await hookChoiceContext.AssignTaskAndWaitForPauseOrCompletion(autoPlayTask)))
                {
                    await hookChoiceContext.GameAction!.CompletionTask;
                }
            }
        }
    }
}

