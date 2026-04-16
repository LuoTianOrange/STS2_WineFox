using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class Scratch() : WineFoxCard(
        0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override bool HasEnergyCostX => true;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardScratch);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var x = ResolveEnergyXValue();
            var hits = 2 * x;
            if (hits <= 0) return;

            var damage = (decimal)(IsUpgraded ? x + 2 : x);

            await DamageCmd.Attack(damage)
                .WithHitCount(hits)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade() { }
    }
}
