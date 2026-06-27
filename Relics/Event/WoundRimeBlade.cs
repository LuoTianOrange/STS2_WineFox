using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics.Event
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class WoundRimeBlade : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Event;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.WoundRimeBladeRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<VulnerablePower>(1m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<VulnerablePower>()];

        public override decimal ModifyBlockAdditive(
            Creature target,
            decimal block,
            ValueProp props,
            CardModel? cardSource,
            CardPlay? cardPlay)
        {
            return target.IsPlayer ? 0m : -block;
        }

        public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
        {
            if (creature.IsPlayer || delta <= 0m)
                return;

            Flash();
            await CreatureCmd.SetCurrentHp(creature, creature.CurrentHp - delta);
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner)
                return;

            var enemies = player.Creature.CombatState?.HittableEnemies.ToList();
            if (enemies == null || enemies.Count == 0)
                return;

            var enemy = player.RunState.Rng.CombatTargets.NextItem(enemies);
            if (enemy == null)
                return;

            Flash();
            await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, DynamicVars.Vulnerable.BaseValue, Owner.Creature, null);
        }
    }
}
