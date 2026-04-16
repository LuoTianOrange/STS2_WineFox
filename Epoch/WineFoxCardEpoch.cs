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
    ///     Card pack epoch.
    /// </summary>
    [RegisterEpoch]
    [RegisterStoryEpoch(typeof(WineFoxModStory))]
    [AutoTimelineSlot(EpochEra.Seeds0)]
    [RegisterEpochCards(
        typeof(AlterPath),
        typeof(EnmergencyRepair),
        typeof(IronZombie),
        typeof(LightAssault),
        typeof(MechanicalSaw),
        typeof(PlantTrees),
        typeof(CrushingWheel),
        typeof(FullAttack),
        typeof(MechanicalDrill),
        typeof(PowerUp),
        typeof(MiningGems),
        typeof(SteamEngine))]
    public class WineFoxCardEpoch : PackDeclaredCardUnlockEpochTemplate
    {
        public override string Id => WineFoxTimelineKeys.CardEpochId;

        public override string StoryId => WineFoxTimelineKeys.TimelineStoryId;
    }
}
