using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class EternalMelodyPlusPower : EternalMelodyRetentionPower
    {
        public override PowerType Type => PowerType.Buff;
    }
}
