using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class SnowBallOverwhelming() : WineFoxCard(
        1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(3m, ValueProp.Move), new IntVar("Hits", 1m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSnowBallOverwhelming);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, nameof(play.Target));

            var hits = DynamicVars["Hits"].IntValue;
            for (var i = 0; i < hits; i++)
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(play.Target)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);

            await PowerCmd.Apply<SnowBallOverwhelmingPower>(Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Hits"].UpgradeValueBy(1m);
        }
    }
}
