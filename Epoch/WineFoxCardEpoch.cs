using MegaCrit.Sts2.Core.Timeline;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Uncommon;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    public class WineFoxCardEpoch : CardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.CardEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;

        public override EpochEra Era => EpochEra.Seeds0;

        public override int EraPosition => 1;

        protected override IEnumerable<Type> CardTypes =>
        [
            // Common
            typeof(AlterPath),
            typeof(EnmergencyRepair),
            typeof(IronZombie),
            typeof(LightAssault),
            typeof(MechanicalSaw),
            typeof(PlantTrees),
            // Uncommon
            typeof(CrushingWheel),
            typeof(FullAttack),
            typeof(MechanicalDrill),
            typeof(PowerUp),
            // Rare
            typeof(MiningGems),
            typeof(SteamEngine),
        ];
    }
}
