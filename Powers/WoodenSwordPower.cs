using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;  // VigorPower
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class WoodenSwordPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => new(
            Const.Paths.WoodenSwordPowerIcon,
            Const.Paths.WoodenSwordPowerIcon);

        public override async Task AfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            Flash();
            await PowerCmd.Apply<VigorPower>(Owner, 4m, Owner, null);
            await PowerCmd.ModifyAmount(this, -1m, null, null);

            if ((decimal)Amount <= 0m)
                await PowerCmd.Remove(this);
        }
    }
}