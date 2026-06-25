using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Upgraded variant of MemoryPower.
    ///     Inherits all tracking logic; generated copy additionally gains Retain.
    /// </summary>
    [RegisterPower]
    public class MemoryPowerUpgraded : MemoryPower
    {
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.MemoryPowerIcon);

        protected override void ConfigureClone(CardModel clone)
        {
            clone.AddKeyword(CardKeyword.Retain);
        }
    }
}
