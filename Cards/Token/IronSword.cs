using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token
{
    public class IronSword() : WineFoxCard(
        0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy,
        showInCardLibrary: false, autoAdd: false)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(14m, ValueProp.Move), new("Echoes", 1m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardIronSword,
            Const.Paths.CardIronSword);

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

            await PowerCmd.Apply<IronSwordPower>(
                Owner.Creature, DynamicVars["Echoes"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Echoes"].UpgradeValueBy(1m);
        }
    }
}