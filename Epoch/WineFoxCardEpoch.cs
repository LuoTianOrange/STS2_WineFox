using STS2_WineFox.Content.Descriptors;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    /// <summary>
    ///     Card pack epoch — gated card types are declared on <see cref="WineFoxContentManifest" /> (pack).
    /// </summary>
    public class WineFoxCardEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.CardEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }
}
