using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Cards.Token.Craft;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class ShieldDexPower : TemporaryDexterityPower, IModPowerAssetOverrides
    {
        public override AbstractModel OriginModel => ModelDb.Card<Shield>();

        public PowerAssetProfile AssetProfile =>
            new(Const.Paths.ShieldDexPowerIcon, Const.Paths.ShieldDexPowerIcon);

        public string? CustomIconPath => AssetProfile.IconPath;

        public string? CustomBigIconPath => AssetProfile.BigIconPath;
    }
}
