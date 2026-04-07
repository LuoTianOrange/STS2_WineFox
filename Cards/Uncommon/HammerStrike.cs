using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class HammerStrike() : WineFoxCard(
        3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(24m, ValueProp.Move),
            new IntVar("Multiplier", 3m),
            ModCardVars.Computed("TotalDamage", 24m, CalcTotalDamage),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardHammerStrike);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var creature = Owner.Creature;

            var totalStrength = creature.Powers
                .Where(p => p is StrengthPower or TemporaryStrengthPower)
                .Sum(p => p.Amount);

            var mult = DynamicVars["Multiplier"].BaseValue;

            var damage = DynamicVars.Damage.BaseValue + (mult - 1m) * totalStrength;

            await DamageCmd.Attack(damage)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Multiplier"].UpgradeValueBy(2m); // 3 → 5
        }
        
        private static decimal CalcTotalDamage(CardModel? card)
        {
            if (card == null) return 0m;
            if (!card.DynamicVars.TryGetValue("Damage", out var dmg)) return 0m;
            if (!card.DynamicVars.TryGetValue("Multiplier", out var mult)) return 0m;
            var creature = card._owner?.Creature;
            if (creature == null) return dmg.BaseValue;

            var totalStrength = creature.Powers
                .Where(p => p is StrengthPower or TemporaryStrengthPower)
                .Sum(p => p.Amount);

            return dmg.BaseValue + mult.BaseValue * totalStrength;
        }
    }
}
