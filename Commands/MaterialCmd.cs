using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Commands
{
    public static class MaterialCmd
    {
        public static async Task GainMaterial<T>(CardModel card, decimal amount)
            where T : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            var owner = card.Owner?.Creature ?? throw new InvalidOperationException("Material source has no owner creature.");
            await GainMaterial<T>(owner, amount);
            await TryTriggerIronPickaxe(owner, card);
        }

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

        public static async Task GainMaterials<TFirst, TSecond>(CardModel card, decimal firstAmount, decimal secondAmount)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            var owner = card.Owner?.Creature ?? throw new InvalidOperationException("Material source has no owner creature.");
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

            await TryTriggerIronPickaxe(owner, card);
        }
        
        public static async Task GainAllMaterials(Creature creature, decimal amount)
        {
            if (creature == null) throw new ArgumentNullException(nameof(creature));

            var stress = creature.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);

            if (stress == null)
            {
                await PowerCmd.Apply<WoodPower>(creature, amount, creature, null);
                await PowerCmd.Apply<StonePower>(creature, amount, creature, null);
                await PowerCmd.Apply<IronPower>(creature, amount, creature, null);
                await PowerCmd.Apply<DiamondPower>(creature, amount, creature, null);
            }
            else
            {
                await PowerCmd.Apply<WoodPower>(creature, amount * 2, creature, null);
                await PowerCmd.Apply<StonePower>(creature, amount * 2, creature, null);
                await PowerCmd.Apply<IronPower>(creature, amount * 2, creature, null);
                await PowerCmd.Apply<DiamondPower>(creature, amount * 2, creature, null);

                await PowerCmd.Apply<StressPower>(creature, -1m, creature, null);
            }
        }
        
        public static async Task LoseMaterials<TFirst, TSecond>(CardModel card, decimal firstAmount, decimal secondAmount)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            var owner = card.Owner?.Creature ?? throw new InvalidOperationException("Material source has no owner creature.");

            var firstPower = owner.Powers.OfType<TFirst>()
                .FirstOrDefault(power => power.Amount >= firstAmount);
            var secondPower = owner.Powers.OfType<TSecond>()
                .FirstOrDefault(power => power.Amount >= secondAmount);

            if (firstPower == null || secondPower == null)
                return;

            await PowerCmd.ModifyAmount(firstPower, -firstAmount, null, card);
            await PowerCmd.ModifyAmount(secondPower, -secondAmount, null, card);
        }

        private static async Task TryTriggerIronPickaxe(Creature creature, CardModel card)
        {
            var power = creature.Powers
                .OfType<IronPickaxePower>()
                .FirstOrDefault(p => p.Amount > 0);

            if (power == null)
                return;

            power.Flash();
            await GainMaterial<IronPower>(creature, 2m);

            await PowerCmd.ModifyAmount(power, -1m, null, card);
            if (power.Amount <= 0m)
                await PowerCmd.Remove(power);
        }
    }
}
