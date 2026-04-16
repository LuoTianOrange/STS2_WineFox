using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Cards.Uncommon;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Scaffolding.Content.Patches;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     预判先机：本回合临时敏捷加成
    /// </summary>
    [RegisterPower]
    public class AnticipateAdvantageDexPower : TemporaryDexterityPower, IModPowerAssetOverrides
    {
        public override AbstractModel OriginModel => ModelDb.Card<AnticipateAdvantage>();

        public PowerAssetProfile AssetProfile =>
            new(Const.Paths.AnticipateAdvantageDexPowerIcon, Const.Paths.AnticipateAdvantageDexPowerIcon);

        public string? CustomIconPath => AssetProfile.IconPath;
        public string? CustomBigIconPath => AssetProfile.BigIconPath;
    }
}
