using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class EasyPeasyPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.EasyPeasyPowerIcon);

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            return player != Owner.Player ? count : count + Amount;
        }

        public override decimal ModifyMaxEnergy(Player player, decimal amount)
        {
            return player != Owner.Player ? amount : amount + Amount;
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            Flash();
            await Task.CompletedTask;
        }
    }
}
