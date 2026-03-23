using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token
{
    public class WoodenSword() : WineFoxCard(
        0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy,
        showInCardLibrary: false, autoAdd: false)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(4m, ValueProp.Move), new("Vigorous", 4m), new("Turns", 2m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardWoodenSword,
            Const.Paths.CardWoodenSword);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var target = play.Target
                         ?? Owner.Creature.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive);

            if (target != null)
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(target)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);

            await PowerCmd.Apply<WoodenSwordPower>(
                Owner.Creature, 1m, Owner.Creature, this);
        }


        protected override void OnUpgrade() { }
    }
}