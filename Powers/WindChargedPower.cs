using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class WindChargedPower : PotionDeathMarkPower
    {
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.WindChargedPowerIcon);

        protected override async Task OnMarkedOwnerDied(PlayerChoiceContext choiceContext, float deathAnimLength)
        {
            var targets = CombatState.HittableEnemies.Where(enemy => enemy != Owner).ToList();
            if (targets.Count == 0)
                return;

            await CreatureCmd.Damage(choiceContext, targets, 20m, ValueProp.Unpowered, Applier, null, null);
        }
    }
}
