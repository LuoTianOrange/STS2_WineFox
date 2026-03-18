using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    public class HandCrank : WineFoxRelic
    {
        // 标记是否有待消耗的应力（用于跨越同步/异步边界）
        private bool _pendingStressConsume;

        public override RelicAssetProfile AssetProfile => new(
            Const.Paths.HandCrankRelicIcon,
            Const.Paths.HandCrankRelicIcon,
            Const.Paths.HandCrankRelicIcon);

        public override RelicRarity Rarity => RelicRarity.Starter;

        // ① 每回合开始时获得 1 层应力
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner)
                return;

            Flash();
            await PowerCmd.Apply<StressPower>(Owner.Creature, 1m, Owner.Creature, null);
        }

        // ② 同步钩子：拦截材料 Power 的给予量，满足条件时翻倍
        public override decimal ModifyPowerAmountGiven(
            PowerModel power,
            Creature giver,
            decimal amount,
            Creature? target,
            CardModel? cardSource)
        {
            if (WineFoxActions.IsManualMaterialBatchActive)
                return amount;

            // 只处理：目标是持有者、Power 是材料类
            if (target != Owner.Creature) return amount;
            if (power is not MaterialPower) return amount;

            // 检查持有者是否有应力
            var stress = Owner.Creature.Powers.OfType<StressPower>().FirstOrDefault();
            if (stress is not { Amount: > 0 }) return amount;

            // 标记待消耗，并将数量翻倍
            _pendingStressConsume = true;
            return amount * 2;
        }

        // ③ 异步钩子：在翻倍之后异步消耗 1 层应力
        public override async Task AfterModifyingPowerAmountGiven(PowerModel power)
        {
            if (WineFoxActions.IsManualMaterialBatchActive)
                return;

            if (!_pendingStressConsume) return;
            if (power is not MaterialPower) return;

            _pendingStressConsume = false; // 先重置，避免 Apply<Stress> 触发再入
            Flash();
            await PowerCmd.Apply<StressPower>(Owner.Creature, -1m, Owner.Creature, null);
        }
    }
}
