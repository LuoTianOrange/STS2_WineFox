using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Upgraded variant of <see cref="MemoryPower" />.
    ///     Inherits all tracking logic; generated copy additionally gains <see cref="CardKeyword.Retain" />.
    /// </summary>
    [RegisterPower]
    public class MemoryPowerUpgraded : MemoryPower
    {
        protected override void ConfigureClone(CardModel clone)
        {
            clone.AddKeyword(CardKeyword.Retain);
        }
    }
}
