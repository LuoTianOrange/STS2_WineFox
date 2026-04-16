using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public class Deployer : HandCrank
    {
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.DeployerRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Stress", 3m)];
    }
}
