using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class NoMoreFalchion() : WineFoxCard(
        1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.IronKeyword];
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(8m, ValueProp.Move), new IntVar("Hits", 1m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNoMoreFalchion);

        // 打出后返回手牌
        protected override (PileType, CardPilePosition) GetResultPileTypeAndPositionForCardPlay()
        {
            var (pileType, position) = base.GetResultPileTypeAndPositionForCardPlay();
            return pileType != PileType.Discard ? (pileType, position) : (PileType.Hand, CardPilePosition.Bottom);
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
                        .FromCard(this, play)
                        .Targeting(target)
                        .WithHitFx("vfx/vfx_attack_slash")
                        .Execute(choiceContext);
            }

            // 消耗1个铁锭，永久+1次数
            var creature = Owner.Creature;
            var ironPower = creature.Powers.OfType<IronPower>().FirstOrDefault(p => p.Amount > 0m);
            if (ironPower != null)
            {
                var consumeEvent = new MaterialConsumeEvent
                {
                    Creature = creature,
                    SourceCard = this,
                    Deltas = [new(typeof(IronPower), 1m)],
                    TotalAmount = 1m,
                };
                await MaterialEventFlow.DispatchBeforeConsume(consumeEvent);
                await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), ironPower, -1m, null, this);
                await MaterialEventFlow.DispatchAfterConsume(consumeEvent);
                await MaterialEventFlow.DispatchAfterResolved(new()
                {
                    Creature = consumeEvent.Creature,
                    SourceCard = consumeEvent.SourceCard,
                    Deltas = consumeEvent.Deltas,
                    TotalAmount = consumeEvent.TotalAmount,
                    Kind = MaterialChangeKind.Consume,
                    AppliedStressMultiplier = false,
                });
                DynamicVars["Hits"].BaseValue += 1m;
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m); // 8 → 10
        }
    }
}
