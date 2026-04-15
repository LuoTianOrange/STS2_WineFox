using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
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

            var target = cardPlay.Target;
            if (target == null || !target.IsAlive) return;
            if (target.Side == Owner.Side) return; // 必须是敌人

            Flash();

            // 在施加之前记录敌人是否已经有缓慢
            var hadSlow = target.Powers.OfType<SlowPower>().Any();

            // 施加 1 层缓慢
            await PowerCmd.Apply<SlowPower>(target, 1m, Owner, null);

            // 若敌人之前已有缓慢，额外 +1 SlowAmount（使本次出牌贡献翻倍为 +2）
            if (hadSlow)
            {
                var slowPower = target.Powers.OfType<SlowPower>().FirstOrDefault();
                if (slowPower != null)
                {
                    ++slowPower.DynamicVars["SlowAmount"].BaseValue;
                    slowPower.InvokeDisplayAmountChanged();
                }
            }
        }
    }
}
