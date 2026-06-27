using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics.Event
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class BleedingHeart : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Event;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.BleedingHeartRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new HealVar(2m),
            new MaxHpVar(7m),
        ];

        public override async Task AfterCombatVictory(CombatRoom room)
        {
            if (!Owner.Creature.IsDead)
            {
                Flash();
                await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
            }

            if (room.RoomType != RoomType.Elite)
                return;

            Flash();
            await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
        }
    }
}
