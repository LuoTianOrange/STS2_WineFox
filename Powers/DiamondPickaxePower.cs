using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class DiamondPickaxePower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.DiamondPickaxePowerIcon);
        
    }
}