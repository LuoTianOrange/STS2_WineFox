using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class WoodPower : MaterialPower
    {
        public override PowerAssetProfile AssetProfile =>
            Icons(Const.Paths.WoodPowerIcon, Const.Paths.WoodPowerBigIcon);
    }
}
