using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class FullAttack() : WineFoxCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Material];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(0m),
            new ExtraDamageVar(4m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) => GetTotalMaterials(card)),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardFullAttack);

        protected override bool IsPlayable => GetTotalMaterials(this) > 0;

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var ownerCreature = Owner?.Creature;
            if (ownerCreature == null) return;
            if (ownerCreature.CombatState is not { } _) return;

            var totalMaterials = await MaterialCmd.ConsumeAllMaterialsForSeries(this, play);

            if (totalMaterials <= 0) return;

            var damage =
                DynamicVars.CalculationBase.BaseValue
                + DynamicVars.ExtraDamage.BaseValue * totalMaterials;

            await DamageCmd.Attack(damage)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }

        private static decimal GetTotalMaterials(CardModel? card)
        {
            var creature = card?.Owner?.Creature;
            if (creature == null) return 0m;

            return creature.Powers
                .OfType<MaterialPower>()
                .Sum(p => p.Amount);
        }
    }
}
