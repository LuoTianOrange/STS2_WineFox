using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.NoMoreFalchion
{
    public class SteelChamber() : WineFoxCard(
        1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(4m, ValueProp.Move), new IntVar("Hits", 1m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSteelChamber);

        protected override PileType GetResultPileType()
        {
            var result = base.GetResultPileType();
            return result != PileType.Discard ? result : PileType.Hand;
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var target = play.Target
                         ?? Owner.Creature.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive);

            if (target != null)
            {
                var hits = DynamicVars["Hits"].IntValue;
                for (var i = 0; i < hits; i++)
                    await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                        .FromCard(this)
                        .Targeting(target)
                        .WithHitFx("vfx/vfx_attack_slash")
                        .Execute(choiceContext);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m); // 4 → 6
        }
    }
}
