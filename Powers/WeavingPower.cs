using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class WeavingPower : PotionDeathMarkPower
    {
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.WeavingPowerIcon);

        protected override async Task OnMarkedOwnerDied(PlayerChoiceContext choiceContext, float deathAnimLength)
        {
            var targets = CombatState.HittableEnemies.Where(enemy => enemy != Owner).ToList();
            if (targets.Count == 0)
                return;

            await PowerCmd.Apply<WeakPower>(choiceContext, targets, 3m, Applier, null);
        }
    }
}
