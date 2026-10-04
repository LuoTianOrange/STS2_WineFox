namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     法术牌：装填进法杖后，由回合结束的「释放」按槽位顺序释放。
    ///     实现者需要提供自己的施放逻辑，并自行读取 <c>context.Modifiers</c> 来应用修正符效果。
    /// </summary>
    public interface IMagicWineFoxSpellCard
    {
        Task CastAsSpell(MagicWineFoxSpellCastContext context);
    }

    /// <summary>
    ///     修正符：装填后不施放，只修改 <see cref="MagicWineFoxSpellModifierState" />，作用于序列中紧随其后的那一张法术。
    ///     若其后没有法术则空放（不做特殊处理）。
    /// </summary>
    public interface IMagicWineFoxSpellModifierCard
    {
        void ApplyModifier(MagicWineFoxSpellModifierState modifiers);
    }

    /// <summary>
    ///     作用于**前一张**法术的修正符：装填顺序里排在它之前、最近的那一张法术会吃到它的修正。
    ///     前面没有法术时空放（不额外处理）。
    /// </summary>
    public interface IMagicWineFoxSpellLookBehindModifierCard : IMagicWineFoxSpellModifierCard
    {
    }

    /// <summary>可选：法术自定义充能球/槽位图标。没有实现时回落到默认图标。</summary>
    public interface IMagicWineFoxSpellIconProvider
    {
        string SpellIconPath { get; }
    }
}
