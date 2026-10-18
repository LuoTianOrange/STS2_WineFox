using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     槽位里一张已装填法术的快照。
    ///     <para>
    ///         <see cref="Card" /> 是打出时创建的克隆，不指向手牌里的那张实体卡，
    ///         因此释放过程不会影响手牌/弃牌堆的原卡。
    ///     </para>
    ///     <para>
    ///         <see cref="Target" /> 记录装填时的目标；目标已死时释放方会回落到当前目标或重选。
    ///     </para>
    ///     <para>
    ///         <see cref="SourceCard" /> 是装填时那张手牌实体卡，供释放阶段对它写入跨回合状态（如伤害修正）。
    ///         自动装回的路径只保留克隆，因此它可能为 null。
    ///     </para>
    /// </summary>
    public sealed record MagicWineFoxSpellSlotSnapshot(
        CardModel Card,
        Creature? Target,
        bool IsModifier,
        int XValue = 0,
        CardModel? SourceCard = null);
}
