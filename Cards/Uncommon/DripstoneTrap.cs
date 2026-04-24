using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class DripstoneTrap() : WineFoxCard(
        2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(2m, ValueProp.Move),
            ModCardVars.Computed("Hits", 0m, CalcHits),
        ];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stone];
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<VulnerablePower>()];
        
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardDripstoneTrap);

        protected override bool IsPlayable
        {
            get
            {
                var creature = Owner?.Creature;
                if (creature == null) return false;

                var stone = creature.Powers.OfType<StonePower>().FirstOrDefault()?.Amount ?? 0;
                return stone > 0;
            }
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner?.Creature;
            if (owner == null) return;
            if (owner.CombatState is not { } combatState) return;

            var stoneAmountDecimal = await MaterialCmd.ConsumeAllMaterialOfTypeForSeries<StonePower>(this, play);
            var totalHits = (int)stoneAmountDecimal;
            if (totalHits <= 0) return;

            for (var i = 0; i < totalHits; i++)
            {
                var target = combatState.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
                if (target == null) break;

                await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
                    .FromCard(this)
                    .Targeting(target)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
                await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, 1m, owner, this);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }

        private static decimal CalcHits(CardModel? card)
        {
            var creature = card?._owner?.Creature;
            if (creature == null) return 0m;

            var stone = creature.Powers.OfType<StonePower>().FirstOrDefault()?.Amount ?? 0;
            return stone;
        }
    }
}
