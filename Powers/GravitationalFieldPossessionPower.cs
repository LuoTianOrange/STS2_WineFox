using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     引力场附身（挂在敌人身上的能力）。
    ///     <para>
    ///         与引力奇点方向相反：其他敌人受到伤害时，持有本能力的敌人才承受同等伤害。
    ///     </para>
    /// </summary>
    [RegisterPower]
    public class GravitationalFieldPossessionPower : WineFoxPower
    {
        private static readonly HashSet<Creature> Redirecting = [];

        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile =>
            Icons(Const.Paths.GravitationalFieldPossessionPowerIcon);

        public override async Task AfterDamageReceived(
            PlayerChoiceContext choiceContext,
            Creature target,
            DamageResult result,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (target == Owner)
                return;

            if (Redirecting.Contains(Owner))
                return;

            var damage = result.UnblockedDamage;
            if (damage <= 0)
                return;

            if (Owner.CombatState is not { } combatState)
                return;

            if (!combatState.HittableEnemies.Contains(target))
                return;

            Flash();
            Redirecting.Add(Owner);

            try
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    Owner,
                    damage,
                    ValueProp.Unpowered,
                    null,
                    null);
            }
            finally
            {
                Redirecting.Remove(Owner);
            }
        }
    }
}
