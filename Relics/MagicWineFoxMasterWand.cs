using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(MagicWineFoxRelicPool))]
    public class MagicWineFoxMasterWand : MagicWineFoxWand
    {
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.MasterStaffIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("SlotCapacity", MagicWineFoxSpellCmd.DefaultCapacity + 3),
            new("CastCount", MagicWineFoxSpellCmd.DefaultCastCount),
            new("PreloadSigils", 1m),
            new("LoadDiscount", MagicWineFoxSpellCmd.FirstLoadDiscount),
        ];
    }
}
