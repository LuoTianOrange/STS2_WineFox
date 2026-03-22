using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class MechanicalSaw() : WineFoxCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stress];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(15m, ValueProp.Move)];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardMechanicalSaw,
            Const.Paths.CardMechanicalSaw);

        protected override bool IsPlayable =>
            Owner.Creature.Powers.OfType<StressPower>().Any(p => (decimal)p.Amount > 0);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;
            
            await PowerCmd.Apply<StressPower>(owner, -1m, owner, this);
            
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(5m); // 15 → 20
        }
    }
}