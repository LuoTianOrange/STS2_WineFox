using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.Craft
{
    [RegisterCard(typeof(WineFoxCraftingCardPool))]
    public class WoodenSword() : WineFoxCard(
        0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(6m, ValueProp.Move), new("Vigorous", 4m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardWoodenSword);

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

            await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
                DynamicVars["Vigorous"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Vigorous"].UpgradeValueBy(2m);
        }
    }
}
