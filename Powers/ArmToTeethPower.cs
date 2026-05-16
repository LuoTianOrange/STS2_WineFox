using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class ArmToTeethPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ArmToTeethPowerIcon);

        public override decimal ModifyDamageMultiplicative(
            Creature? target,
            decimal amount,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (!props.IsPoweredAttack() ||
                cardSource == null ||
                !IsOwnerOrOwnedPet(dealer) ||
                !HasActivePlating() ||
                !HasActiveStrength())
                return 1m;

            if (Amount <= 0m)
                return 1m;

            Flash();
            return Amount;
        }

        private bool IsOwnerOrOwnedPet(Creature? dealer)
        {
            return ReferenceEquals(dealer, Owner) || Owner.Pets.Contains(dealer);
        }

        private bool HasActivePlating()
        {
            var plating = Owner.Powers.OfType<PlatingPower>().FirstOrDefault();
            return plating != null && plating.Amount > 0m;
        }

        private bool HasActiveStrength()
        {
            var strength = Owner.Powers.OfType<StrengthPower>().FirstOrDefault();
            return strength != null && strength.Amount > 0m;
        }
    }
}
