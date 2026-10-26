using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class SpellExperimentPower : WineFoxPower
    {
        private bool _releasedThisTurn;

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.None;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.SpellExperimentPowerIcon);

        protected override Task OnAfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature == Owner)
                _releasedThisTurn = false;

            return Task.CompletedTask;
        }

        public async Task TryReleaseWhenStaffFull(
            PlayerChoiceContext choiceContext,
            Player owner,
            MagicWineFoxSpellSlotPower power)
        {
            if (_releasedThisTurn)
                return;

            if (owner.Creature != Owner)
                return;

            if (power.Slots.Count == 0 || power.Slots.Any(slot => slot == null))
                return;

            _releasedThisTurn = true;
            Flash();

            await MagicWineFoxSpellCmd.CastAll(choiceContext, owner, null, null);
        }
    }
}
