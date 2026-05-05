using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Combat.Magic
{
    /// <summary>
    ///     Participates in WineFox magic damage calculation.
    ///     This pipeline is intentionally separate from the base game's generic damage hooks so that
    ///     magic cards can opt out of Strength and similar modifiers while still supporting curated buffs.
    /// </summary>
    public interface IMagicDamageModifier
    {
        decimal ModifyMagicDamageAdditive(
            Creature? target,
            decimal baseAmount,
            Creature dealer,
            CardModel cardSource);
    }
}

