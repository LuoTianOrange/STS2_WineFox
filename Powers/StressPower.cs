using MegaCrit.Sts2.Core.Entities.Powers;

namespace STS2_WineFox.Powers;

public class StressPower : WineFoxPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

}