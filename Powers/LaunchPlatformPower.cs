using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Applied by LaunchPlatform (弹射置物台).
    ///     First time you consume stress each turn, gain 1 energy.
    /// </summary>
    [RegisterPower]
    public class LaunchPlatformPower : WineFoxPower, IStressConsumeListener
    {
        private bool _usedThisTurn;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.LaunchPlatformPowerIcon);

        protected override Task OnAfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature == Owner)
                _usedThisTurn = false;
            return Task.CompletedTask;
        }

        public async Task OnStressConsumed(Creature creature)
        {
            if (creature != Owner) return;
            if (_usedThisTurn) return;

            _usedThisTurn = true;
            Flash();
            var player = Owner.Player;
            if (player != null)
                await PlayerCmd.GainEnergy(1m, player);
        }
    }
}

