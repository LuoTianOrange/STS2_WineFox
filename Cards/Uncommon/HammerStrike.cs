using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class HammerStrike() : WineFoxCard(
        4, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(14m),
            new ExtraDamageVar(1m),
            new IntVar("Multiplier", 3m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(StrengthBonusScaledByMultiplier),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardHammerStrike);

        public override Task AfterCardRetained(CardModel card)
        {
            if (!ReferenceEquals(card, this)) return Task.CompletedTask;

            EnergyCost.AddThisCombat(-1);
            return Task.CompletedTask;
        }

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
            DynamicVars.CalculationBase.UpgradeValueBy(1m);
            DynamicVars["Multiplier"].UpgradeValueBy(2m); // 3 → 5
        }

        private static decimal StrengthAmount(Creature? creature)
        {
            return creature == null ? 0m : creature.GetPowerAmount<StrengthPower>();
        }

        private static decimal StrengthBonusScaledByMultiplier(CardModel card, Creature? _)
        {
            if (!card.DynamicVars.TryGetValue("Multiplier", out var mult)) return 0m;
            var creature = card._owner?.Creature;
            if (creature == null) return 0m;
            return (mult.BaseValue - 1m) * StrengthAmount(creature);
        }
    }
}
