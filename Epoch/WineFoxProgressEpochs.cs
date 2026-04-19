using MegaCrit.Sts2.Core.Timeline;
using STS2_WineFox.Cards.Ancient;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Deleted;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Uncommon;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    /// <summary>
    ///     Placeholder — first clear as WineFox (vanilla-style “character 5” milestone).
    /// </summary>
    [RegisterEpoch]
    [RegisterStoryEpoch(typeof(WineFoxModStory))]
    [AutoTimelineSlot(EpochEra.Blight2)]
    [RegisterEpochCards(
        typeof(Traditionalist),
        typeof(RecordPlayer),
        typeof(WorkbenchBackpack))]
    public sealed class WineFoxVictoryEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.VictoryEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }

    /// <summary>
    ///     Placeholder — 15 elite wins as WineFox.
    /// </summary>
    [RegisterEpoch]
    [RegisterStoryEpoch(typeof(WineFoxModStory))]
    [AutoTimelineSlot(EpochEra.Prehistoria2)]
    [RegisterEpochCards(
        typeof(AllItem),
        typeof(LessHoliday),
        typeof(Alternator))]
    public sealed class WineFoxEliteEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.EliteMilestoneEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }

    /// <summary>
    ///     Placeholder — 15 boss wins as WineFox.
    /// </summary>
    [RegisterEpoch]
    [RegisterStoryEpoch(typeof(WineFoxModStory))]
    [AutoTimelineSlot(EpochEra.Flourish0)]
    [RegisterEpochCards(
        typeof(CobblestoneGenerator),
        typeof(NetheritePickaxe),
        typeof(RiclearPowerPlant))]
    public sealed class WineFoxBossEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.BossMilestoneEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }

    /// <summary>
    ///     Placeholder — ascension 1 win as WineFox.
    /// </summary>
    [RegisterEpoch]
    [RegisterStoryEpoch(typeof(WineFoxModStory))]
    [AutoTimelineSlot(EpochEra.Invitation5)]
    [RegisterEpochCards(
        typeof(SlashBladeWood),
        typeof(DripstoneTrap),
        typeof(Forging))]
    public sealed class WineFoxAscensionOneEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.AscensionOneEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }
}
