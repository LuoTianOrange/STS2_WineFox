using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
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
        public static async Task<decimal> ResolveCardMaterialAmount(
            Creature creature,
            CardModel card,
            decimal baseAmount,
            bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(creature);
            ArgumentNullException.ThrowIfNull(card);

            if (baseAmount <= 0m)
                return 0m;

            var stress = applyStress && await TryTriggerStressPower(creature) ? 2m : 1m;
            return baseAmount * stress;
        }

        public static async Task GainMaterial<T>(CardModel card, decimal amount, bool applyStress = true)
            where T : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            if (amount <= 0m)
                return;

            await CommitCardMaterialGains(card, [(typeof(T), amount)], applyStress);
        }

        public static async Task GainMaterial<T>(Creature creature, decimal amount, CardModel? sourceCard = null,
            bool applyStress = true)
            where T : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(creature);

            if (amount <= 0m)
                return;

            await CommitCardMaterialGains(creature, sourceCard, [(typeof(T), amount)], applyStress);
        }

        public static async Task GainMaterials<TFirst, TSecond>(CardModel card, decimal firstAmount,
            decimal secondAmount, bool applyStress = true)
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

            await CommitCardMaterialGains(card, gains, applyStress);
        }

        public static async Task GainMaterials<TFirst, TSecond>(Creature creature, decimal firstAmount,
            decimal secondAmount, CardModel? sourceCard = null, bool applyStress = true)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(creature);

            var gains = new List<(Type Type, decimal Amount)>(2);
            if (firstAmount > 0m)
                gains.Add((typeof(TFirst), firstAmount));
            if (secondAmount > 0m)
                gains.Add((typeof(TSecond), secondAmount));

            if (gains.Count == 0)
                return;

            await CommitCardMaterialGains(creature, sourceCard, gains, applyStress);
        }

        public static async Task GainMaterials(CardModel card, IEnumerable<(Type Type, decimal Amount)> materials,
            bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(card);
            ArgumentNullException.ThrowIfNull(materials);

            var gains = materials.Where(m => m.Amount > 0m).ToList();
            if (gains.Count == 0)
                return;

            await CommitCardMaterialGains(card, gains, applyStress);
        }

        public static async Task GainMaterials(Creature creature, IEnumerable<(Type Type, decimal Amount)> materials,
            CardModel? sourceCard = null, bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(creature);
            ArgumentNullException.ThrowIfNull(materials);

            var gains = materials.Where(m => m.Amount > 0m).ToList();
            if (gains.Count == 0)
                return;

            await CommitCardMaterialGains(creature, sourceCard, gains, applyStress);
        }

        /// <summary>
        ///     From a played card: one stress check for the whole batch, then one iron pickaxe check.
        /// </summary>
        public static async Task GainAllMaterials(CardModel card, decimal amountPerType, bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(card);

            if (amountPerType <= 0m)
                return;

            var gains = MaterialPowerRegistry.RegisteredMaterialTypes
                .Select(t => (t, amountPerType))
                .ToList();

            await CommitCardMaterialGains(card, gains, applyStress);
        }

        public static async Task GainAllMaterials(Creature creature, decimal amountPerType, bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(creature);

            if (amountPerType <= 0m)
                return;

            if (applyStress && await TryTriggerStressPower(creature))
                amountPerType *= 2m;

            await MaterialPowerRegistry.ApplyAll(creature, amountPerType, null);
            await TryTriggerIronPickaxe(creature, null);
        }

        public static async Task LoseMaterial<T>(CardModel card, decimal amount)
            where T : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            var owner = card.Owner?.Creature ??
                        throw new InvalidOperationException("Material source has no owner creature.");

            await PowerCmd.Apply<T>(owner, -amount, owner, card);
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
                .FirstOrDefault(power => power.Amount >= 0);
            var secondPower = owner.Powers.OfType<TSecond>()
                .FirstOrDefault(power => power.Amount >= 0);

            if (firstPower == null || secondPower == null)
                return;

            await PowerCmd.Apply<TFirst>(owner, -firstAmount, owner, card);
            await PowerCmd.Apply<TSecond>(owner, -secondAmount, owner, card);
        }

        private static Task CommitCardMaterialGains(CardModel card,
            IReadOnlyList<(Type Type, decimal Amount)> gains, bool applyStress = true)
        {
            return CommitCardMaterialGains(card.Owner.Creature, card, gains, applyStress);
        }

        private static async Task CommitCardMaterialGains(Creature creature, CardModel? card,
            IReadOnlyList<(Type Type, decimal Amount)> gains, bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(creature);

            var filtered = gains.Where(g => g.Amount > 0m).ToList();
            if (filtered.Count == 0)
                return;

            var mult = applyStress && await TryTriggerStressPower(creature) ? 2m : 1m;

            foreach (var (type, amount) in filtered)
                await MaterialPowerRegistry.Apply(creature, type, amount * mult, card);

            await TryTriggerIronPickaxe(creature, card);
        }

        private static async Task<bool> TryTriggerStressPower(Creature creature)
        {
            var stress = creature.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);
            if (stress == null)
                return false;

            await PowerCmd.Apply<StressPower>(creature, -1m, creature, null);
            return true;
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
