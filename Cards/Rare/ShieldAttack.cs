using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Cards.Rare
{
    public class ShieldAttack() : WineFoxCard(
        0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        private static readonly AttachedState<CardModel, ShieldAttackSeriesState> SeriesStates =
            new(() => new());

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(2m, ValueProp.Move | ValueProp.Unpowered),
                ModCardVars.Computed("TotalDamage", 0m, CalcTotalDamage),
            ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardShieldAttack);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var creature = owner.Creature;

            var combatState = creature.CombatState;
            if (combatState is null) return;

            var state = SeriesStates.GetOrCreate(this);
            var damage = state.Damage;
            var blockToLose = 0m;

            if (play.IsFirstInSeries)
            {
                var block = creature.Block;
                damage = block * DynamicVars.Damage.BaseValue;
                state.Damage = damage;
                blockToLose = block;
            }

            await DamageCmd.Attack(damage)
                .FromCard(this)
                .TargetingAllOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Unpowered()
                .Execute(choiceContext);

            if (blockToLose > 0m) await CreatureCmd.LoseBlock(creature, blockToLose);

            await CardPileCmd.Draw(choiceContext, 1m, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1m);
        }

        private static decimal CalcTotalDamage(CardModel? card)
        {
            if (card == null) return 0m;
            if (!card.DynamicVars.TryGetValue("Damage", out var damageVar)) return 0m;
            var creature = card._owner?.Creature;
            if (creature == null) return 0m;
            return creature.Block * damageVar.BaseValue;
        }
        
        private sealed class ShieldAttackSeriesState
        {
            public decimal Damage { get; set; }
        }
    }
}
