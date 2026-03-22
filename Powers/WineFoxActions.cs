using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Powers
{
    public static class WineFoxActions
    {
        /// <summary>
        ///     获得单种材料。若有应力，消耗 1 层并将获得量翻倍。
        /// </summary>
        public static async Task GainMaterial<T>(CardModel card, decimal amount)
            where T : MaterialPower
        {
            var owner = card.Owner.Creature;
            var stress = owner.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);

            if (stress == null)
            {
                await PowerCmd.Apply<T>(owner, amount, owner, card);
                return;
            }

            await PowerCmd.Apply<T>(owner, amount * 2, owner, card);
            await PowerCmd.Apply<StressPower>(owner, -1m, owner, card);
        }

        /// <summary>
        ///     批量获得两种材料。若有应力，消耗 1 层并将获得量翻倍。
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
                return;
            }

            await PowerCmd.Apply<TFirst>(owner, firstAmount * 2, owner, card);
            await PowerCmd.Apply<TSecond>(owner, secondAmount * 2, owner, card);
            await PowerCmd.Apply<StressPower>(owner, -1m, owner, card);
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
    }
}
