using MegaCrit.Sts2.Core.Timeline;
using STS2_WineFox.Character;
using STS2RitsuLib.Timeline.Scaffolding;

namespace STS2_WineFox.Epoch
{
    public class WineFoxCharacterEpoch : CharacterUnlockEpochTemplate<WineFox>
    {
        public override string Id => "WineFoxCardEpoch";
        public override EpochEra Era => EpochEra.Seeds0;
        public override int EraPosition => 0;

        protected override IEnumerable<Type> ExpansionEpochTypes =>
        [
            typeof(WineFoxCardEpoch),
        ];
    }
}
