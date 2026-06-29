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
using STS2RitsuLib.Combat.Healing;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics.Event
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class WoundRimeBlade : WineFoxRelic, IHealHookListener
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
            return IsEnemy(target) ? -block : 0m;
        }

        public decimal ModifyHealAmount(HealContext context, decimal amount)
        {
            var creature = context.Creature;
            if (!IsEnemy(creature) || creature.IsDead || amount <= 0m)
                return amount;

            Flash();
            return 0m;
        }

        private bool IsEnemy(Creature creature)
        {
            if (creature.IsPlayer || creature.Player == Owner)
                return false;

            return creature.CombatState?.HittableEnemies.Contains(creature) == true;
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
