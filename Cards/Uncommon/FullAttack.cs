using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class FullAttack() : WineFoxCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            ModCardVars.Computed("Damage", 4m, CalcDamage),
            ModCardVars.Computed("Hits", 0m, CalcHits),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardFullAttack);

        protected override bool IsPlayable
        {
            get
            {
                var wood = Owner.Creature.Powers.OfType<WoodPower>().FirstOrDefault()?.Amount ?? 0;
                var stone = Owner.Creature.Powers.OfType<StonePower>().FirstOrDefault()?.Amount ?? 0;
                return wood + stone > 0;
            }
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var woodPower = owner.Powers.OfType<WoodPower>().FirstOrDefault();
            var stonePower = owner.Powers.OfType<StonePower>().FirstOrDefault();
            var woodAmount = woodPower?.Amount ?? 0;
            var stoneAmount = stonePower?.Amount ?? 0;
            var totalHits = woodAmount + stoneAmount;

            if (totalHits <= 0) return;

            if (woodPower != null && woodAmount > 0)
                await PowerCmd.ModifyAmount(woodPower, -(decimal)woodAmount, null, this);
            if (stonePower != null && stoneAmount > 0)
                await PowerCmd.ModifyAmount(stonePower, -(decimal)stoneAmount, null, this);
            WineFoxActions.MaterialConsumeCountThisTurn++;

            await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
                .WithHitCount(totalHits)
                .FromCard(this)
                .TargetingRandomOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }

        private static decimal CalcDamage(CardModel? card)
        {
            if (card == null) return 4m;
            if (!card.DynamicVars.TryGetValue("Damage", out var dynamicVar)) return 4m;

            var creature = card._owner?.Creature;
            if (creature == null) return dynamicVar.BaseValue;

            var hits = CalcHits(card);
            return hits > 0
                ? dynamicVar.BaseValue * hits
                : dynamicVar.BaseValue;
        }

        private static decimal CalcHits(CardModel? card)
        {
            var creature = card?._owner?.Creature;
            if (creature == null) return 0m;

            var wood = creature.Powers.OfType<WoodPower>().FirstOrDefault()?.Amount ?? 0;
            var stone = creature.Powers.OfType<StonePower>().FirstOrDefault()?.Amount ?? 0;
            return wood + stone;
        }
    }
}
