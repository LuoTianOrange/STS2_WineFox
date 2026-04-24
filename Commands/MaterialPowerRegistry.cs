using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Commands
{
    /// <summary>
    ///     Extensible registry of material power types for <see cref="MaterialCmd" /> batch gains.
    ///     WineFox registers its defaults in <see cref="Main.Initialize" />.
    ///     Addon mods should call <see cref="Register{T}" /> from their mod initializer; registration order defines
    ///     <c>GainAll</c> apply order. Duplicate registrations for the same type are ignored.
    /// </summary>
    public static class MaterialPowerRegistry
    {
        private static readonly Lock Gate = new();

        private static readonly List<Entry> Entries = [];

        /// <summary>
        ///     Snapshot of registered types in registration order.
        /// </summary>
        public static IReadOnlyList<Type> RegisteredMaterialTypes
        {
            get
            {
                lock (Gate)
                {
                    return Entries.Select(e => e.PowerType).ToArray();
                }
            }
        }

        /// <summary>
        ///     Registers WineFox built-in materials. Safe to call multiple times.
        /// </summary>
        public static void RegisterWineFoxDefaults()
        {
            Register<WoodPower>();
            Register<StonePower>();
            Register<IronPower>();
            Register<DiamondPower>();
        }

        /// <summary>
        ///     Registers a <see cref="MaterialPower" /> subtype for material gain pipelines.
        /// </summary>
        public static void Register<T>()
            where T : MaterialPower
        {
            var type = typeof(T);
            lock (Gate)
            {
                if (Entries.Exists(e => e.PowerType == type))
                    return;

                if (type.IsAbstract)
                    throw new ArgumentException($"{type} must be a concrete {nameof(MaterialPower)} subtype.");

                Entries.Add(new(type,
                    async (creature, amount, sourceCard) =>
                        await PowerCmd.Apply<T>(new ThrowingPlayerChoiceContext(), creature, amount, creature, sourceCard)));
            }
        }

        internal static async Task Apply(Creature creature, Type materialPowerType, decimal amount,
            CardModel? sourceCard)
        {
            if (amount == 0m)
                return;

            Func<Creature, decimal, CardModel?, Task>? apply;
            lock (Gate)
            {
                apply = Entries.FirstOrDefault(e => e.PowerType == materialPowerType)?.ApplyAsync;
            }

            if (apply == null)
                throw new InvalidOperationException(
                    $"Material power type not registered: {materialPowerType.FullName}. " +
                    $"Call {nameof(MaterialPowerRegistry)}.{nameof(Register)}<T>() from your mod initializer.");

            await apply(creature, amount, sourceCard);
        }

        internal static async Task ApplyAll(Creature creature, decimal amountEach, CardModel? sourceCard)
        {
            IReadOnlyList<Type> types;
            lock (Gate)
            {
                types = Entries.Select(e => e.PowerType).ToArray();
            }

            foreach (var t in types)
                await Apply(creature, t, amountEach, sourceCard);
        }

        private sealed record Entry(Type PowerType, Func<Creature, decimal, CardModel?, Task> ApplyAsync);
    }
}
