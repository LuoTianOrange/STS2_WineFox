using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class StonePower : MaterialPower
    {
        public override PowerAssetProfile AssetProfile => new(
            Const.Paths.StonePowerIcon,
            Const.Paths.StonePowerBigIcon);
    }
}
