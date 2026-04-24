using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class NoMoreFalchion() : WineFoxCard(
        1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Iron];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(8m, ValueProp.Move), new IntVar("Hits", 1m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNoMoreFalchion);

        // 打出后返回手牌
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
                    await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                        .WithHitCount(hits)
                        .FromCard(this)
                        .Targeting(target)
                        .WithHitFx("vfx/vfx_attack_slash")
                        .Execute(choiceContext);
            }

            // 消耗1个铁锭，永久+1次数
            var ironPower = Owner.Creature.Powers.OfType<IronPower>().FirstOrDefault(p => p.Amount > 0m);
            if (ironPower != null)
            {
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), ironPower, -1m, null, this);
                DynamicVars["Hits"].BaseValue += 1m;
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m); // 8 → 10
        }
    }
}
