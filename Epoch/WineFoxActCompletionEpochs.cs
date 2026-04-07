using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    /// <summary>
    ///     Act 1 boss cleared — aligns with vanilla <c>…2_EPOCH</c> id from
    ///     <see cref="WineFoxTimelineKeys.ModCharacterEntryUpper" />.
    /// </summary>
    public sealed class WineFoxAct1Epoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.ActCompletionEpochId(0);

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }

    /// <summary>
    ///     Act 2 boss cleared — <c>…3_EPOCH</c>.
    /// </summary>
    public sealed class WineFoxAct2Epoch : PackDeclaredRelicUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.ActCompletionEpochId(1);

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }

    /// <summary>
    ///     Act 3 boss cleared — <c>…4_EPOCH</c>.
    /// </summary>
    public sealed class WineFoxAct3Epoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.ActCompletionEpochId(2);

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }
}
