using MegaCrit.Sts2.Core.Timeline;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Uncommon;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    /// <summary>
    ///     Act 1 boss cleared — aligns with vanilla <c>…2_EPOCH</c> id from
    ///     <see cref="WineFoxTimelineKeys.ModCharacterEntryUpper" />.
    /// </summary>
    [RegisterEpoch]
    [RegisterStoryEpoch(typeof(WineFoxModStory))]
    [AutoTimelineSlot(EpochEra.Blight1)]
    [RegisterEpochCards(
        typeof(EasyPeasy),
        typeof(FoxBite),
        typeof(ShieldAttack))]
    public sealed class WineFoxAct1Epoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.ActCompletionEpochId(0);

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }

    /// <summary>
    ///     Act 2 boss cleared — <c>…3_EPOCH</c>.
    /// </summary>
    [RegisterEpoch]
    [RegisterStoryEpoch(typeof(WineFoxModStory))]
    [AutoTimelineSlot(EpochEra.Peace0)]
    [RegisterEpochRelicsFromPool(typeof(WineFoxRelicPool))]
    public sealed class WineFoxAct2Epoch : PackDeclaredRelicUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.ActCompletionEpochId(1);

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }

    /// <summary>
    ///     Act 3 boss cleared — <c>…4_EPOCH</c>.
    /// </summary>
    [RegisterEpoch]
    [RegisterStoryEpoch(typeof(WineFoxModStory))]
    [AutoTimelineSlot(EpochEra.Seeds2)]
    [RegisterEpochCards(
        typeof(SpinningHand),
        typeof(VacantDomain),
        typeof(MaidSupport))]
    public sealed class WineFoxAct3Epoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.ActCompletionEpochId(2);

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }
}
