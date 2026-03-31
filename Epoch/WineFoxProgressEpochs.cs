using MegaCrit.Sts2.Core.Timeline;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    /// <summary>
    ///     Placeholder — first clear as WineFox (vanilla-style “character 5” milestone).
    /// </summary>
    public sealed class WineFoxVictoryEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.VictoryEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Blight2;

        public override int EraPosition => 2;
    }

    /// <summary>
    ///     Placeholder — 15 elite wins as WineFox.
    /// </summary>
    public sealed class WineFoxEliteEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.EliteMilestoneEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Prehistoria2;

        public override int EraPosition => 2;
    }

    /// <summary>
    ///     Placeholder — 15 boss wins as WineFox.
    /// </summary>
    public sealed class WineFoxBossEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.BossMilestoneEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Flourish0;

        public override int EraPosition => 2;
    }

    /// <summary>
    ///     Placeholder — ascension 1 win as WineFox.
    /// </summary>
    public sealed class WineFoxAscensionOneEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.AscensionOneEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Invitation5;

        public override int EraPosition => 3;
    }
}
