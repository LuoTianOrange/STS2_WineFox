using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Commands
{
    public static class MaterialCmd
    {
        /// <summary>
        ///     Resolves material amount for a delayed effect granted by a card (e.g. Plant): applies stress
        ///     doubling and consumption now, like <see cref="DodgeAndRoll" /> locking block for next turn.
        ///     The returned value should be stored on a power and granted later without going through
        ///     <see cref="MaterialCmd" /> again.
        /// </summary>
        public static async Task<decimal> ResolveCardMaterialAmountWithStressAsync(
            Creature creature,
            CardModel card,
            decimal baseAmount)
        {
            ArgumentNullException.ThrowIfNull(creature);
            ArgumentNullException.ThrowIfNull(card);

            if (baseAmount <= 0m)
                return 0m;

            var stress = creature.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);
            if (stress == null)
                return baseAmount;

            await PowerCmd.Apply<StressPower>(creature, -1m, creature, card);
            return baseAmount * 2m;
        }

        public static async Task GainMaterial<T>(CardModel card, decimal amount)
            where T : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            if (amount <= 0m)
                return;

            await CommitCardMaterialGains(card, [(typeof(T), amount)]);
        }

        public static async Task GainMaterials<TFirst, TSecond>(CardModel card, decimal firstAmount,
            decimal secondAmount)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            var gains = new List<(Type Type, decimal Amount)>(2);
            if (firstAmount > 0m)
                gains.Add((typeof(TFirst), firstAmount));
            if (secondAmount > 0m)
                gains.Add((typeof(TSecond), secondAmount));

            if (gains.Count == 0)
                return;

            await CommitCardMaterialGains(card, gains);
        }

        /// <summary>
        ///     From a played card: one stress check for the whole batch, then one iron pickaxe check.
        /// </summary>
        public static async Task GainAllMaterials(CardModel card, decimal amountPerType)
        {
            ArgumentNullException.ThrowIfNull(card);

            if (amountPerType <= 0m)
                return;

            var gains = MaterialPowerRegistry.RegisteredMaterialTypes
                .Select(t => (t, amountPerType))
                .ToList();

            await CommitCardMaterialGains(card, gains);
        }

        /// <summary>
        ///     From powers / non-card sources: no stress; still runs one iron pickaxe check after all materials.
        /// </summary>
        public static async Task GainAllMaterialsFromNonCard(Creature creature, decimal amountPerType)
        {
            ArgumentNullException.ThrowIfNull(creature);

            if (amountPerType <= 0m)
                return;

            await MaterialPowerRegistry.ApplyAll(creature, amountPerType, null);
            await TryTriggerIronPickaxe(creature, null);
        }

        public static async Task LoseMaterials<TFirst, TSecond>(CardModel card, decimal firstAmount,
            decimal secondAmount)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            var owner = card.Owner?.Creature ??
                        throw new InvalidOperationException("Material source has no owner creature.");

            var firstPower = owner.Powers.OfType<TFirst>()
                .FirstOrDefault(power => power.Amount >= firstAmount);
            var secondPower = owner.Powers.OfType<TSecond>()
                .FirstOrDefault(power => power.Amount >= secondAmount);

            if (firstPower == null || secondPower == null)
                return;

            await PowerCmd.ModifyAmount(firstPower, -firstAmount, null, card);
            await PowerCmd.ModifyAmount(secondPower, -secondAmount, null, card);
        }

        private static async Task CommitCardMaterialGains(CardModel card,
            IReadOnlyList<(Type Type, decimal Amount)> gains)
        {
            var owner = card.Owner?.Creature ??
                        throw new InvalidOperationException("Material source has no owner creature.");

            var filtered = gains.Where(g => g.Amount > 0m).ToList();
            if (filtered.Count == 0)
                return;

            var stress = owner.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);
            var mult = stress != null ? 2m : 1m;

            if (stress != null)
                await PowerCmd.Apply<StressPower>(owner, -1m, owner, card);

            foreach (var (type, amount) in filtered)
                await MaterialPowerRegistry.Apply(owner, type, amount * mult, card);

            await TryTriggerIronPickaxe(owner, card);
        }

        private static async Task TryTriggerIronPickaxe(Creature creature, CardModel? card)
        {
            var power = creature.Powers
                .OfType<IronPickaxePower>()
                .FirstOrDefault(p => p.Amount > 0);

            if (power == null)
                return;

            power.Flash();

            await PowerCmd.Apply<IronPower>(creature, 3m, creature, card);

            await PowerCmd.ModifyAmount(power, -1m, null, card);
            if (power.Amount <= 0m)
                await PowerCmd.Remove(power);
        }
    }
}
