using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Combat.Magic
{
    /// <summary>
    ///     Participates in WineFox magic block calculation.
    /// </summary>
    public interface IMagicBlockModifier
    {
        decimal ModifyMagicBlockAdditive(
            Creature defender,
            decimal baseAmount,
            CardModel cardSource);
    }
}

