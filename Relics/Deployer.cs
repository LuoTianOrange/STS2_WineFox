using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    public class Deployer : HandCrank
    {
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.DeployerRelicIcon);
        
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Stress", 3m),];
    }
}