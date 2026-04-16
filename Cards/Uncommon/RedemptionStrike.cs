using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class RedemptionStrike() : WineFoxCard(
        1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(8m, ValueProp.Move), new HealVar(2m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRedemptionStrike);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            var creature = Owner.Creature;
            var healAmount = DynamicVars.Heal.BaseValue;

            await CreatureCmd.Heal(creature, healAmount);

            var combatState = creature.CombatState;
            if (combatState == null) return;

            foreach (var ally in combatState.GetTeammatesOf(creature).Where(c => c.IsAlive && c != creature))
                await CreatureCmd.Heal(ally, healAmount);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Heal.UpgradeValueBy(1m);
        }
    }
}
