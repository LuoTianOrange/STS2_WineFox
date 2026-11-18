using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class InviteTheFlamePower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.InviteTheFlamePowerIcon);

        public override async Task AfterDamageReceived(
            PlayerChoiceContext choiceContext,
            Creature target,
            DamageResult result,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (target != Owner)
                return;

            if (result.UnblockedDamage <= 0)
                return;

            if (Amount <= 0m)
                return;

            if (CombatState is not { } combatState)
                return;

            var enemies = combatState.HittableEnemies.Where(enemy => enemy.IsAlive).ToList();
            if (enemies.Count == 0)
                return;

            var enemy = combatState.RunState.Rng.CombatTargets.NextItem(enemies);
            if (enemy == null)
                return;

            Flash();

            await PowerCmd.Apply<BurningPower>(
                choiceContext,
                enemy,
                Amount,
                Owner,
                cardSource);
        }
    }
}
