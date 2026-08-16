using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    // [RegisterRelic(typeof(MagicWineFoxRelicPool))]
    public class Shrine : MaidBeacon
    {
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.ShrineRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(3),
            new CardsVar(3),
        ];
    }
}
