using MegaCrit.Sts2.Core.Timeline;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    [RegisterEpoch]
    [RegisterStoryEpoch(typeof(WineFoxModStory))]
    [AutoTimelineSlotBeforeColumn(EpochEra.Seeds0)]
    [RequireAllCardsInPool(typeof(WineFoxCardPool))]
    public class WineFoxCharacterEpoch : CharacterUnlockEpochTemplate<WineFox>
    {
        public override string Id => WineFoxTimelineKeys.CharacterEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        protected override IEnumerable<Type> ExpansionEpochTypes =>
        [
            typeof(WineFoxCardEpoch),
            typeof(WineFoxAct1Epoch),
            typeof(WineFoxAct2Epoch),
            typeof(WineFoxAct3Epoch),
            typeof(WineFoxVictoryEpoch),
            typeof(WineFoxEliteEpoch),
            typeof(WineFoxBossEpoch),
            typeof(WineFoxAscensionOneEpoch),
        ];
    }
}
