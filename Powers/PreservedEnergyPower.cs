using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class PreservedEnergyPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.PreservedEnergyPowerIcon);

        public override bool ShouldPlayerResetEnergy(Player player)
        {
            return player != Owner.Player;
        }

        public override bool ShouldClearBlock(Creature creature)
        {
            return !ReferenceEquals(Owner, creature);
        }

        public override Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
        {
            if (!ReferenceEquals(this, preventer))
                return Task.CompletedTask;

            Flash();
            return Task.CompletedTask;
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext,
            Player player)
        {
            if (player.Creature != Owner)
                return;

            if (Amount <= 1)
            {
                await PowerCmd.Remove(this);
                return;
            }

            await PowerCmd.ModifyAmount(choiceContext, this, -1m, null, null);
        }
    }
}
