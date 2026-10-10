using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;

namespace STS2_WineFox.Cards
{
    internal static class WineFoxCardVarFactory
    {
        internal static Func<CardModel?, CardPreviewMode, Creature?, bool, decimal> StressDoubledDynamicVar(string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            return (card, _, _, runGlobalHooks) =>
            {
                if (card == null)
                    return 0m;

                if (!card.DynamicVars.TryGetValue(key, out var dynamicVar))
                    return 0m;

                if (!runGlobalHooks || card.CombatState == null)
                    return dynamicVar.BaseValue;

                var hasStress = card.Owner.Creature.Powers
                    .OfType<StressPower>()
                    .Any(power => power.Amount > 0);

                return hasStress ? dynamicVar.BaseValue * 2m : dynamicVar.BaseValue;
            };
        }

        internal static DynamicVar BlockAmountVar(decimal baseValue, ValueProp props = ValueProp.Move)
        {
            return new BlockVar(baseValue, props);
        }
    }
}
