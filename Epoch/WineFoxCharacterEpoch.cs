using MegaCrit.Sts2.Core.Timeline;
using STS2_WineFox.Character;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    public class WineFoxCharacterEpoch : CharacterUnlockEpochTemplate<WineFox>
    {
        public override string Id => WineFoxTimelineKeys.CharacterEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Seeds0;

        public override int EraPosition => 0;

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
