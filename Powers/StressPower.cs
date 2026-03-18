using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class StressPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        
        public override PowerAssetProfile AssetProfile => new(
            Const.Paths.StressPowerIcon,
            Const.Paths.StressPowerBigIcon);
    }
}
