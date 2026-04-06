using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Utils;

internal static class MilkCleanseHelper
{
    internal static async Task Cleanse(Creature creature, Creature? applier, CardModel? cardSource)
    {
        var applierCreature = applier ?? creature;

        foreach (var p in creature.Powers.ToList().Where(Eligible))
        {
            if (p is ITemporaryPower && p.TypeForCurrentAmount == PowerType.Debuff)
                await RevertAndRemoveTemporary(p, creature, applierCreature, cardSource);
        }

        foreach (var p in creature.Powers.ToList().Where(Eligible))
        {
            if (p is not ITemporaryPower && p.TypeForCurrentAmount == PowerType.Debuff)
                await PowerCmd.Remove(p);
        }
    }

    private static bool Eligible(PowerModel p) => p is not MaterialPower;

    private static async Task RevertAndRemoveTemporary(
        PowerModel power,
        Creature owner,
        Creature applierCreature,
        CardModel? cardSource)
    {
        switch (power)
        {
            case TemporaryStrengthPower tsp:
            {
                var amt = tsp.Amount;
                var sign = tsp.TypeForCurrentAmount == PowerType.Debuff ? -1 : 1;
                await PowerCmd.Remove(tsp);
                await PowerCmd.Apply<StrengthPower>(owner, -sign * amt, applierCreature, cardSource, silent: true);
                return;
            }
            case TemporaryDexterityPower tdp:
            {
                var amt = tdp.Amount;
                var sign = tdp.TypeForCurrentAmount == PowerType.Debuff ? -1 : 1;
                await PowerCmd.Remove(tdp);
                await PowerCmd.Apply<DexterityPower>(owner, -sign * amt, applierCreature, cardSource, silent: true);
                return;
            }
            case TemporaryFocusPower tfp:
            {
                var amt = tfp.Amount;
                var sign = tfp.TypeForCurrentAmount == PowerType.Debuff ? -1 : 1;
                await PowerCmd.Remove(tfp);
                await PowerCmd.Apply<FocusPower>(owner, -sign * amt, applierCreature, cardSource, silent: true);
                return;
            }
            default:
                await PowerCmd.Remove(power);
                return;
        }
    }
}
