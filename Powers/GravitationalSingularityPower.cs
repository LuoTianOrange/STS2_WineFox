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
    /// 引力奇点（挂在敌人身上的能力）。
    /// <para>
    /// 该敌人每次受到伤害时，它之外的其他敌人受到等同的伤害。
    /// </para>
    /// </summary>
    [RegisterPower]
    public class GravitationalSingularityPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Debuff;

        // Single：数值隐藏且恒为 1，重复施加不会叠层（本能力只需要"存在"这一个状态）。
        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.GravitationalSingularityPowerIcon);

        /// <summary>
        /// 正在转嫁的生物集合。
        /// <para>
        /// 必须是静态的：如果 A、B 两个敌人各挂了一份本能力，
        /// A 受伤 → 转嫁 B → B 受伤 → 又转嫁回 A…… 会互相弹射成死循环。
        /// 用实例字段只能挡住"自己这一份"，挡不住另一份的回弹。
        /// </para>
        /// </summary>
        private static readonly HashSet<Creature> Redirecting = [];

        public override async Task AfterDamageReceived(
            PlayerChoiceContext choiceContext,
            Creature target,
            DamageResult result,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            // 只处理挂在本能力持有者身上的那次伤害。
            if (target != Owner)
                return;

            // 这次伤害本身就是转嫁来的，不再外传（否则会无限弹射）。
            if (Redirecting.Contains(Owner))
                return;

            // 按实际掉血转嫁（被格挡的部分不算），且为 0 时无事发生。
            var damage = result.UnblockedDamage;
            if (damage <= 0)
                return;

            if (Owner.CombatState is not { } combatState)
                return;

            // 必须用 HittableEnemies（只含敌方生物）。
            // GetOpponentsOf 会把玩家自己也算进对手，导致转嫁伤害打到玩家身上。
            var others = combatState.HittableEnemies
                .Where(other => other != Owner && other.IsAlive)
                .ToList();

            if (others.Count == 0)
                return;

            Flash();
            Redirecting.Add(Owner);

            try
            {
                foreach (var other in others)
                {
                    await CreatureCmd.Damage(
                        choiceContext,
                        other,
                        damage,
                        // 只有 Unpowered：转嫁伤害仍会被目标自己的格挡抵挡，但不吃增伤/减伤修饰。
                        ValueProp.Unpowered,
                        null,
                        null);
                }
            }
            finally
            {
                Redirecting.Remove(Owner);
            }
        }
    }
}
