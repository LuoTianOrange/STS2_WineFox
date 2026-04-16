using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class LightAssault() : WineFoxCard(
        0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Material];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(13m),
            new ExtraDamageVar(-1m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(MaterialPenaltyForLightAssault),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLightAssault);

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
            DynamicVars.CalculationBase.UpgradeValueBy(3m);
        }

        private static decimal MaterialPenaltyForLightAssault(CardModel card, Creature? _)
        {
            var b = card.DynamicVars.CalculationBase.BaseValue;
            var creature = card._owner?.Creature;
            if (creature == null) return 0m;

            var totalMaterials = creature.Powers
                .OfType<MaterialPower>()
                .Sum(p => (decimal)p.Amount);

            var perTwo = Math.Floor(totalMaterials / 2m);
            return Math.Min(perTwo, b);
        }
    }
}
