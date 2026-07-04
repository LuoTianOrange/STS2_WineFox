using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class AdaptToCircumstances() : WineFoxCard(
        1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(8m, ValueProp.Move),
            new IntVar("Debuff", 1m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAdaptToCircumstances);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var target = play.Target;
            var creature = Owner.Creature;
            var debuffAmount = DynamicVars["Debuff"].BaseValue;

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            if (target.Monster?.IntendsToAttack == true)
                await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, debuffAmount, creature, this);
            else
                await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, debuffAmount, creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);   // 8 → 10
            DynamicVars["Debuff"].UpgradeValueBy(1m); // 1 → 2
        }
    }
}
