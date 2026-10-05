using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Mechanics
{
    /// <summary>预览里一个条目在释放流程中的角色。</summary>
    public enum MagicWineFoxSpellPreviewKind
    {
        Cast,
        Modifier,
        Reverse,
        Replay,
        Kept,
    }

    /// <summary>预览分段：一段连续的结算（正向 / 倒序重放 / 保留）。</summary>
    public enum MagicWineFoxSpellPreviewSegmentKind
    {
        Forward,
        Replay,
        Kept,
    }

    /// <summary>预览里的一个条目：哪张牌、什么角色、单次伤害与次数。</summary>
    public readonly record struct MagicWineFoxSpellPreviewStep(
        CardModel Card,
        MagicWineFoxSpellPreviewKind Kind,
        decimal DamagePerHit,
        int Hits);

    /// <summary>一段结算链。</summary>
    public sealed record MagicWineFoxSpellPreviewSegment(
        MagicWineFoxSpellPreviewSegmentKind Kind,
        IReadOnlyList<MagicWineFoxSpellPreviewStep> Steps);

    /// <summary>一次释放的预览结果：分段的执行顺序 + 预估总伤害。</summary>
    public sealed record MagicWineFoxSpellReleasePreview(
        IReadOnlyList<MagicWineFoxSpellPreviewSegment> Segments,
        decimal TotalDamage);
}
