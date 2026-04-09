using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class GoldenArmorPower : WineFoxPower
    {
        private decimal _pendingBufferedDamage;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.GoldenArmorPowerIcon);

        // 在伤害处理前，如果拥有 Buffer，则记录即将受到的伤害
        public override Task BeforeDamageReceived(
            PlayerChoiceContext choiceContext,
            Creature target,
            decimal amount,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (target != Owner) return Task.CompletedTask;
            // 仅在拥有 BufferPower 时记录（否则没有拦截的意义）
            if (!Owner.Powers.OfType<BufferPower>().Any(p => p.Amount > 0))
                return Task.CompletedTask;

            // 估算 Block 减免后的 HP 伤害
            var postBlockDamage = Math.Max(amount - Owner.Block, 0m);
            if (postBlockDamage > 0m)
                _pendingBufferedDamage = postBlockDamage;

            return Task.CompletedTask;
        }

        // BufferPower 被消耗时（Amount 减少）触发治疗
        public override async Task AfterPowerAmountChanged(
            PowerModel power,
            decimal amount,
            Creature? applier,
            CardModel? cardSource)
        {
            if (power.Owner != Owner) return;
            if (power is not BufferPower) return;
            if (amount >= 0m) return;              // 仅关心减少
            if (_pendingBufferedDamage <= 0m) return;

            Flash();
            var healAmount = _pendingBufferedDamage;
            _pendingBufferedDamage = 0m;
            await CreatureCmd.Heal(Owner, healAmount);
        }
    }
}
