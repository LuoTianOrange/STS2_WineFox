using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.Craft
{
    [RegisterCard(typeof(WineFoxCraftingCardPool))]
    public class Nothing() : WineFoxCard(
        0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy
    )
    {
        protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
        protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1m, ValueProp.Move)];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNothing);

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
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(1m);
        }
    }
}
