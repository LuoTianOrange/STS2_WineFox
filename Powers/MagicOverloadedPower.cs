using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;
using STS2_WineFox.Combat.Magic;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class MagicOverloadedPower : WineFoxPower, IMagicDamageModifier, IMagicBlockModifier
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.MagicOverloadedPowerIcon);

        public decimal ModifyMagicDamageAdditive(
            Creature? target,
            decimal baseAmount,
            Creature dealer,
            CardModel cardSource)
        {
            if (dealer != Owner) return 0m;
            if (!cardSource.IsMagic()) return 0m;

            var chant = Owner.GetPowerAmount<ChantPower>();
            return CalculateExtraChantFromPercent(chant, Amount);
        }

        public decimal ModifyMagicBlockAdditive(
            Creature defender,
            decimal baseAmount,
            CardModel cardSource)
        {
            if (defender != Owner) return 0m;
            if (!cardSource.IsMagic()) return 0m;

            var chant = Owner.GetPowerAmount<ChantPower>();
            return CalculateExtraChantFromPercent(chant, Amount);
        }

        public static decimal CalculateExtraChantFromPercent(decimal chantAmount, decimal percent)
        {
            if (chantAmount == 0m || percent == 0m) return 0m;
            return Math.Floor(chantAmount * percent / 100m);
        }
    }
}

