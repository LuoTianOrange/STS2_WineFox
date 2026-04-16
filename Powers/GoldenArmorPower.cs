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
    public class GoldenArmorPower : WineFoxPower
    {
        private decimal _pendingBufferedDamage;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.GoldenArmorPowerIcon);

        public override Task BeforeDamageReceived(
            PlayerChoiceContext choiceContext,
            Creature target,
            decimal amount,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (target != Owner) return Task.CompletedTask;
            if (!Owner.Powers.OfType<BufferPower>().Any(p => p.Amount > 0))
                return Task.CompletedTask;

            var postBlockDamage = Math.Max(amount - Owner.Block, 0m);
            if (postBlockDamage > 0m)
                _pendingBufferedDamage = postBlockDamage;

            return Task.CompletedTask;
        }

        public override async Task AfterPowerAmountChanged(
            PowerModel power,
            decimal amount,
            Creature? applier,
            CardModel? cardSource)
        {
            if (power.Owner != Owner) return;
            if (power is not BufferPower) return;
            if (amount >= 0m) return;
            if (_pendingBufferedDamage <= 0m) return;

            Flash();
            var healAmount = _pendingBufferedDamage;
            _pendingBufferedDamage = 0m;
            await CreatureCmd.Heal(Owner, healAmount);
        }
    }
}
