using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class IronPower : MaterialPower
    {
        public override PowerAssetProfile AssetProfile => new(
            Const.Paths.IronPowerIcon,
            Const.Paths.IronPowerIcon);
    }
}
