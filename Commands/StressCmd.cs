using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Commands
{
    /// <summary>
    ///     Listener interface for stress consumption events.
    /// </summary>
    public interface IStressConsumeListener
    {
        Task OnStressConsumed(Creature creature);
    }

    /// <summary>
    ///     Centralised helper for consuming stress and broadcasting the event
    ///     to any <see cref="IStressConsumeListener" /> powers on the creature.
    /// </summary>
    public static class StressCmd
    {
        /// <summary>
        ///     Tries to consume one stack of <see cref="StressPower" /> from <paramref name="creature" />.
        ///     Returns <c>true</c> if stress was consumed and fires <see cref="IStressConsumeListener" /> events.
        /// </summary>
        public static async Task<bool> ConsumeOne(Creature creature, CardModel? source)
        {
            var stress = creature.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);
            if (stress == null)
                return false;

            await PowerCmd.Apply<StressPower>(new ThrowingPlayerChoiceContext(), creature, -1m, creature, source);

            var listeners = creature.Powers.OfType<IStressConsumeListener>().ToList();
            foreach (var listener in listeners)
                await listener.OnStressConsumed(creature);

            return true;
        }
    }
}

