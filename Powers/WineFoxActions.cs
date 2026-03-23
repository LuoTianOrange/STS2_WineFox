using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Powers
{
    public static class WineFoxActions
    {
        public static bool IsSwordEchoing { get; set; }
        
        /// <summary>
        ///     获得单种材料（来源为卡牌）。若有应力则翻倍；触发铁镐效果。
        /// </summary>
        public static async Task GainMaterial<T>(CardModel card, decimal amount)
            where T : MaterialPower
        {
            await GainMaterial<T>(card.Owner.Creature, amount);
            await TryTriggerIronPickaxe(card.Owner.Creature, card);
        }

        /// <summary>
        ///     获得单种材料（来源为 Power 时使用，不触发铁镐）。
        ///     若有应力，消耗 1 层并将获得量翻倍。
        /// </summary>
        public static async Task GainMaterial<T>(Creature creature, decimal amount)
            where T : MaterialPower
        {
            var stress = creature.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);

            if (stress == null)
            {
                await PowerCmd.Apply<T>(creature, amount, creature, null);
                return;
            }

            await PowerCmd.Apply<T>(creature, amount * 2, creature, null);
            await PowerCmd.Apply<StressPower>(creature, -1m, creature, null);
        }

        /// <summary>
        ///     批量获得两种材料（来源为卡牌）。若有应力则翻倍；触发铁镐效果一次。
        /// </summary>
        public static async Task GainMaterials<TFirst, TSecond>(CardModel card, decimal firstAmount,
            decimal secondAmount)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            var owner = card.Owner.Creature;
            var stress = owner.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);

            if (stress == null)
            {
                await PowerCmd.Apply<TFirst>(owner, firstAmount, owner, card);
                await PowerCmd.Apply<TSecond>(owner, secondAmount, owner, card);
            }
            else
            {
                await PowerCmd.Apply<TFirst>(owner, firstAmount * 2, owner, card);
                await PowerCmd.Apply<TSecond>(owner, secondAmount * 2, owner, card);
                await PowerCmd.Apply<StressPower>(owner, -1m, owner, card);
            }

            // 无论给了几种材料，每张卡只触发一次铁镐
            await TryTriggerIronPickaxe(owner, card);
        }

        /// <summary>
        ///     消耗材料的统一入口。applier 传 null，跳过 ModifyPowerAmountGiven 钩子。
        /// </summary>
        public static async Task LostMaterial<TFirst, TSecond>(CardModel card, decimal firstAmount,
            decimal secondAmount)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            var owner = card.Owner.Creature;

            var firstPower = owner.Powers.OfType<TFirst>()
                .FirstOrDefault(power => power.Amount >= firstAmount);
            var secondPower = owner.Powers.OfType<TSecond>()
                .FirstOrDefault(power => power.Amount >= secondAmount);

            if (firstPower == null || secondPower == null) return;

            await PowerCmd.ModifyAmount(firstPower, -firstAmount, null, card);
            await PowerCmd.ModifyAmount(secondPower, -secondAmount, null, card);
        }

        /// <summary>
        ///     卡牌触发材料获得后，尝试触发铁镐效果。
        ///     使用 Creature 重载获得铁，不会再次触发铁镐，避免无限递归。
        /// </summary>
        private static async Task TryTriggerIronPickaxe(Creature creature, CardModel card)
        {
            var power = creature.Powers
                .OfType<IronPickaxePower>()
                .FirstOrDefault(p => p.Amount > 0);
            if (power == null) return;

            power.Flash();
            await GainMaterial<IronPower>(creature, 2m); // Creature 重载，不再触发铁镐

            await PowerCmd.ModifyAmount(power, -1m, null, card);
            if (power.Amount <= 0m)
                await PowerCmd.Remove(power);
        }
    }
}
