using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    public class TotemofUndying : WineFoxRelic
    {
        public bool _wasUsed;

        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.TotemofUndyingRelicIcon);
        public override RelicRarity Rarity => RelicRarity.Rare;
        public override bool IsUsedUp => _wasUsed;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new HealVar(35m)];

        [SavedProperty]
        public bool WasUsed
        {
            get => _wasUsed;
            set
            {
                AssertMutable();
                _wasUsed = value;
                if (!IsUsedUp) return;
                Status = RelicStatus.Disabled;
            }
        }

        public override bool ShouldDieLate(Creature creature)
            => creature != Owner.Creature || WasUsed;

        public override async Task AfterPreventingDeath(Creature creature)
        {
            Flash();
            WasUsed = true;

            await CreatureCmd.Heal(
                creature,
                Math.Max(1m, creature.MaxHp * (DynamicVars.Heal.BaseValue / 100m)));

            await PowerCmd.Apply<RegenPower>(creature, 10m, creature, null);

            await CreatureCmd.GainBlock(creature, 15m, ValueProp.Unpowered, null);
        }
    }
}
