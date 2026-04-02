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
            if (ownerCreature.CombatState is not { } combatState) return;

            var woodPower = ownerCreature.Powers.OfType<WoodPower>().FirstOrDefault();
            var stonePower = ownerCreature.Powers.OfType<StonePower>().FirstOrDefault();
            var ironPower = ownerCreature.Powers.OfType<IronPower>().FirstOrDefault();
            var diamondPower = ownerCreature.Powers.OfType<DiamondPower>().FirstOrDefault();
            var woodAmount = woodPower?.Amount ?? 0;
            var stoneAmount = stonePower?.Amount ?? 0;
            var ironAmount = ironPower?.Amount ?? 0;
            var diamondAmount = diamondPower?.Amount ?? 0;
            var totalMaterials = woodAmount + stoneAmount + ironAmount + diamondAmount;

            if (totalMaterials <= 0) return;

            var damage =
                DynamicVars.CalculationBase.BaseValue
                + DynamicVars.ExtraDamage.BaseValue * totalMaterials;

            if (woodPower != null && woodAmount > 0)
                await PowerCmd.ModifyAmount(woodPower, -(decimal)woodAmount, null, this);
            if (stonePower != null && stoneAmount > 0)
                await PowerCmd.ModifyAmount(stonePower, -(decimal)stoneAmount, null, this);
            if (ironPower != null && ironAmount > 0)
                await PowerCmd.ModifyAmount(ironPower, -(decimal)ironAmount, null, this);
            if (diamondPower != null && diamondAmount > 0)
                await PowerCmd.ModifyAmount(diamondPower, -(decimal)diamondAmount, null, this);

            CraftCmd.RecordMaterialConsume(ownerCreature);

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

            var wood = creature.Powers.OfType<WoodPower>().FirstOrDefault()?.Amount ?? 0;
            var stone = creature.Powers.OfType<StonePower>().FirstOrDefault()?.Amount ?? 0;
            var iron = creature.Powers.OfType<IronPower>().FirstOrDefault()?.Amount ?? 0;
            var diamond = creature.Powers.OfType<DiamondPower>().FirstOrDefault()?.Amount ?? 0;
            return wood + stone + iron + diamond;
        }
    }
}
