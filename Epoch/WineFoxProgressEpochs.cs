using MegaCrit.Sts2.Core.Timeline;
using STS2_WineFox.Cards.Ancient;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Uncommon;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    /// <summary>Placeholder — first clear as WineFox (vanilla-style “character 5” milestone).</summary>
    public sealed class WineFoxVictoryEpoch : CardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.VictoryEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Blight2;

        public override int EraPosition => 2;

        protected override IEnumerable<Type> CardTypes =>
        [
            typeof(Traditionalist),
            typeof(RecordPlayer),
            typeof(WorkbenchBackpack),
        ];
    }

    /// <summary>Placeholder — 15 elite wins as WineFox.</summary>
    public sealed class WineFoxEliteEpoch : CardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.EliteMilestoneEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Prehistoria2;

        public override int EraPosition => 2;

        protected override IEnumerable<Type> CardTypes =>
        [
            typeof(AllItem),
            typeof(LessHoliday),
            typeof(Alternator),
        ];
    }

    /// <summary>Placeholder — 15 boss wins as WineFox.</summary>
    public sealed class WineFoxBossEpoch : CardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.BossMilestoneEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Flourish0;

        public override int EraPosition => 2;

        protected override IEnumerable<Type> CardTypes =>
        [
            typeof(CobblestoneGenerator),
            typeof(NetheritePickaxe),
            typeof(RiclearPowerPlant),
        ];
    }

    /// <summary>Placeholder — ascension 1 win as WineFox.</summary>
    public sealed class WineFoxAscensionOneEpoch : CardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.AscensionOneEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Invitation5;

        public override int EraPosition => 3;

        protected override IEnumerable<Type> CardTypes =>
        [
            typeof(SlashBladeWood),
            typeof(DripstoneTrap),
            typeof(Forging),
        ];
    }
}
