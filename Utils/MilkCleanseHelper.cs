using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Utils
{
    internal static class MilkCleanseHelper
    {
        internal static async Task Cleanse(Creature creature, Creature? applier, CardModel? cardSource)
        {
            var applierCreature = applier ?? creature;

            foreach (var p in creature.Powers.ToList().Where(Eligible))
                if (p is ITemporaryPower && p.TypeForCurrentAmount == PowerType.Debuff)
                    await RevertAndRemoveTemporary(p, creature, applierCreature, cardSource);

            foreach (var p in creature.Powers.ToList().Where(Eligible))
                if (p is not ITemporaryPower && p.TypeForCurrentAmount == PowerType.Debuff)
                    await PowerCmd.Remove(p);
        }

        private static bool Eligible(PowerModel p)
        {
            return p is not MaterialPower;
        }

        private static async Task RevertAndRemoveTemporary(
            PowerModel power,
            Creature owner,
            Creature applierCreature,
            CardModel? cardSource)
        {
            if (power is not ITemporaryPower temporaryPower)
            {
                await PowerCmd.Remove(power);
                return;
            }

            var internalPower = temporaryPower.InternallyAppliedPower.ToMutable();
            var amount = power.Amount;
            var revertAmount = internalPower.GetTypeForAmount(amount) == PowerType.Debuff
                ? -amount
                : amount;

            await PowerCmd.Remove(power);
            await PowerCmd.Apply(
                new ThrowingPlayerChoiceContext(),
                internalPower,
                owner,
                revertAmount,
                applierCreature,
                cardSource,
                true);
        }
    }
}
