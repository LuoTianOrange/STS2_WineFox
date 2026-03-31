using STS2_WineFox.Epoch;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Character
{
    public class WineFoxModStory : ModStoryTemplate
    {
        protected override string StoryKey => "winefox-story";

        protected override IEnumerable<Type> EpochTypes =>
        [
            typeof(WineFoxCharacterEpoch),
            typeof(WineFoxCardEpoch),
        ];
    }
}
