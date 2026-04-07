using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class HammerStrike() : WineFoxCard(
        3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(24m),
            new ExtraDamageVar(1m),
            new IntVar("Multiplier", 3m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(StrengthBonusScaledByMultiplier),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardHammerStrike);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            await DamageCmd.Attack(DynamicVars.CalculatedDamage)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Multiplier"].UpgradeValueBy(2m); // 3 → 5
        }

        private static decimal StrengthAmount(Creature? creature) =>
            creature == null ? 0m : (decimal)creature.GetPowerAmount<StrengthPower>();

        private static decimal StrengthBonusScaledByMultiplier(CardModel card, Creature? _)
        {
            if (!card.DynamicVars.TryGetValue("Multiplier", out var mult)) return 0m;
            var creature = card._owner?.Creature;
            if (creature == null) return 0m;
            return (mult.BaseValue - 1m) * StrengthAmount(creature);
        }
    }
}
