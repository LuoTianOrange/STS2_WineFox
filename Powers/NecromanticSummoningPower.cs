using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     【死灵召唤】的第二段效果：**每回合结束时**，若奥斯提存活，
    ///     由奥斯提对所有敌人造成伤害。
    ///     <para>
    ///         伤害由本能力自己的 <c>OstyDamage</c> 变量提供（不占用卡牌变量）；
    ///         攻击走原版 <c>AttackCommand.FromOsty</c>，因此受原版相关修正影响。
    ///     </para>
    /// </summary>
    [RegisterPower]
    public class NecromanticSummoningPower : WineFoxPower
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new OstyDamageVar(6m, ValueProp.Move)
        ];

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.None;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.NecromanticSummoningPowerIcon);

        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (side != Owner.Side)
                return;

            if (Owner.Player is not { } player || player.Osty == null)
                return;

            if (Osty.CheckMissingWithAnim(player))
                return;

            if (Owner.CombatState is not { } combatState)
                return;

            var damage = DynamicVars.OstyDamage.BaseValue;
            if (damage <= 0m)
                return;

            Flash();

            await DamageCmd.Attack(damage)
                .FromOsty(player.Osty, null, null)
                .TargetingAllOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
                .Execute(choiceContext);
        }
    }
}

