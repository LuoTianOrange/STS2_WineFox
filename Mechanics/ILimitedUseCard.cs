using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     有限使用次数的卡牌。次数在每场战斗内累计，用尽后由卡牌自行进入消耗牌堆。
    /// </summary>
    public interface ILimitedUseCard
    {
        /// <summary>本场战斗允许的使用次数。</summary>
        int MaxUses { get; }

        /// <summary>剩余可用次数；无限使用时返回 <see cref="int.MaxValue" />。</summary>
        int UsesRemaining { get; }

        /// <summary>是否处于无限使用状态（由卡牌自身或遗物决定）。</summary>
        bool IsUnlimitedUses { get; }

        /// <summary>消耗一次使用次数，返回是否因此用尽。</summary>
        bool TryConsumeUse();
    }

    /// <summary>
    ///     使用次数的修正来源。遗物与能力实现本接口即可参与计算：
    ///     返回无限使用，或在本场上限基础上加减。
    /// </summary>
    public interface ILimitedUseModifier
    {
        /// <summary>是否让该卡牌不再消耗次数。</summary>
        bool GrantsUnlimitedUses(CardModel card);

        /// <summary>在本场上限基础上修正允许的使用次数；不参与时原样返回 <paramref name="limit" />。</summary>
        int ModifyUsesLimit(CardModel card, int limit);
    }
}
