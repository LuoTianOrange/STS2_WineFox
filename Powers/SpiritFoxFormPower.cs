using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class SpiritFoxFormPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.SpiritFoxFormPowerIcon);

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            // 只响应本角色打出的攻击牌
            if (cardPlay.Card?.Owner?.Creature != Owner) return;
            if (cardPlay.Card.Type != CardType.Attack) return;

            var targets = GetAttackTargets(cardPlay).ToArray();
            if (targets.Length == 0) return;

            Flash();

            foreach (var target in targets)
            {
                // 在施加之前记录敌人是否已经有缓慢
                var hadSlow = target.Powers.OfType<SlowPower>().Any();

                await PowerCmd.Apply<SlowPower>(target, Amount, Owner, null);

                // 若敌人之前已有缓慢，额外 +1 SlowAmount（使本次出牌贡献翻倍为 +2）
                if (!hadSlow) continue;

                var slowPower = target.Powers.OfType<SlowPower>().FirstOrDefault();
                if (slowPower is null) continue;

                ++slowPower.DynamicVars["SlowAmount"].BaseValue;
                slowPower.InvokeDisplayAmountChanged();
            }
        }

        private IEnumerable<Creature> GetAttackTargets(CardPlay cardPlay)
        {
            if (cardPlay.Target is { IsAlive: true } singleTarget && singleTarget.Side != Owner.Side)
            {
                yield return singleTarget;
                yield break;
            }

            if (cardPlay.Card.TargetType != TargetType.AllEnemies || Owner.CombatState is null) yield break;

            foreach (var enemy in Owner.CombatState.HittableEnemies)
                if (enemy.IsAlive && enemy.Side != Owner.Side)
                    yield return enemy;
        }
    }
}
