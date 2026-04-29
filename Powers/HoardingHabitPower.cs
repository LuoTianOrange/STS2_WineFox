using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class HoardingHabitPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.HoardingHabitPowerIcon);

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            await MaterialCmd.GainMaterials<WoodPower, StonePower>(
                Owner, Amount, Amount, null, false);

            await PowerCmd.Remove(this);
        }
    }
}
