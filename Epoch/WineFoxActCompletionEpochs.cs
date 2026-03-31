using MegaCrit.Sts2.Core.Timeline;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Uncommon;
using STS2_WineFox.Relics;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    /// <summary>
    ///     Act 1 boss cleared — aligns with vanilla <c>…2_EPOCH</c> id from
    ///     <see cref="WineFoxTimelineKeys.ModCharacterEntryUpper" />.
    /// </summary>
    public sealed class WineFoxAct1Epoch : CardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.ActCompletionEpochId(0);

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Blight1;

        public override int EraPosition => 1;

        protected override IEnumerable<Type> CardTypes =>
        [
            typeof(EasyPeasy),
            typeof(FoxBite),
            typeof(ShieldAttack),
        ];
    }

    /// <summary>Act 2 boss cleared — <c>…3_EPOCH</c>.</summary>
    public sealed class WineFoxAct2Epoch : RelicUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.ActCompletionEpochId(1);

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Seeds3;

        public override int EraPosition => 0;

        protected override IEnumerable<Type> RelicTypes =>
        [
            typeof(HandCrank),
            typeof(MaidBackpack),
            typeof(Deployer),
        ];
    }

    /// <summary>Act 3 boss cleared — <c>…4_EPOCH</c>.</summary>
    public sealed class WineFoxAct3Epoch : CardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.ActCompletionEpochId(2);

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Seeds2;

        public override int EraPosition => 1;

        protected override IEnumerable<Type> CardTypes =>
        [
            typeof(SpinningHand),
            typeof(VacantDomain),
            typeof(MaidSupport),
        ];
    }
}
