using STS2_WineFox.Epoch;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Character
{
    /// <summary>
    ///     WineFox timeline story.
    /// </summary>
    [RegisterStory]
    public class WineFoxModStory : ModStoryTemplate
    {
        protected override string StoryKey => WineFoxTimelineKeys.TimelineStoryId;
    }
}
