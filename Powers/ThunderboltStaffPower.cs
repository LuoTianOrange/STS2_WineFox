using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class ThunderboltStaffPower : WineFoxPower
    {
        private decimal _pendingDamage;

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ThunderboltStaffPowerIcon);

        public override decimal ModifyDamageAdditive(
            Creature? target,
            decimal amount,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource,
            CardPlay? cardPlay)
        {
            if (target != Owner || amount <= 0m)
                return 0m;

            if (CombatState.CurrentSide != Owner.Side)
                return 0m;

            if (dealer != null && dealer != Owner)
                return 0m;

            _pendingDamage += amount;
            return -amount;
        }

        public override async Task BeforeDamageReceived(
            PlayerChoiceContext choiceContext,
            Creature target,
            decimal amount,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (target != Owner || _pendingDamage <= 0m)
                return;

            var reflected = (int)_pendingDamage;
            _pendingDamage = 0m;

            if (reflected <= 0)
                return;

            var enemies = CombatState.HittableEnemies.Where(enemy => !enemy.IsDead).ToList();
            if (enemies.Count == 0)
                return;

            Flash();

            foreach (var enemy in enemies)
                await CreatureCmd.Damage(
                    choiceContext, enemy, reflected, ValueProp.Unpowered | ValueProp.SkipHurtAnim, Owner, null, null);
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext,
            Player player)
        {
            _pendingDamage = 0m;
            await ChargePower.Gain(choiceContext, Owner, Amount, null);
        }

        public override Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            MegaCrit.Sts2.Core.Combat.CombatSide side,
            IEnumerable<Creature> participants)
        {
            _pendingDamage = 0m;
            return Task.CompletedTask;
        }
    }
}
