using STS2_WineFox.Epoch;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Character
{
    /// <summary>
    ///     WineFox timeline story; epoch order is registered via
    ///     <see>
    ///         <cref>TimelineColumnPackEntry{WineFoxModStory}</cref>
    ///     </see>
    ///     .
    /// </summary>
    public class WineFoxModStory : ModStoryTemplate
    {
        protected override string StoryKey => WineFoxTimelineKeys.TimelineStoryId;
    }
}
