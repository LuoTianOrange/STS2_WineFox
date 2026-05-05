using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Combat.Magic;
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

        internal static DynamicVar ChantDamageVar(
            string name,
            decimal baseValue,
            Func<CardModel?, CardPreviewMode, Creature?, bool, Creature?>? targetResolver = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            return ModCardVars.Computed(
                name,
                baseValue,
                (card, target) => ResolveMagicDamageAfterModifiers(
                    card,
                    name,
                    CardPreviewMode.None,
                    target,
                    runGlobalHooks: true,
                    targetResolver),
                (card, previewMode, target, runGlobalHooks) =>
                    ResolveMagicDamageAfterModifiers(
                        card,
                        name,
                        previewMode,
                        target,
                        runGlobalHooks,
                        targetResolver));
        }

        internal static decimal ResolveChantDamageForPlay(CardModel card, string key, Creature? target)
        {
            return ResolveMagicDamageAfterModifiers(
                card,
                key,
                CardPreviewMode.None,
                target,
                runGlobalHooks: true,
                targetResolver: null);
        }

        private static decimal ResolveMagicDamageAfterModifiers(
            CardModel? card,
            string key,
            CardPreviewMode previewMode,
            Creature? previewTarget,
            bool runGlobalHooks,
            Func<CardModel?, CardPreviewMode, Creature?, bool, Creature?>? targetResolver)
        {
            if (card == null) return 0m;
            if (!card.DynamicVars.TryGetValue(key, out var damageVar)) return 0m;

            var dealer = card._owner?.Creature;
            var baseDamage = damageVar.BaseValue;
            if (dealer == null || !runGlobalHooks)
                return Math.Max(0m, baseDamage);

            var target = targetResolver?.Invoke(card, previewMode, previewTarget, runGlobalHooks) ?? previewTarget;
            return MagicDamage.Resolve(card, baseDamage, target);
        }

        internal static DynamicVar BlockAmountVar(decimal baseValue, ValueProp props = ValueProp.Move)
        {
            return new BlockVar(baseValue, props);
        }

        internal static DynamicVar PowerAmountVar<TPower>(decimal baseValue)
            where TPower : PowerModel
        {
            return new PowerVar<TPower>(baseValue);
        }

        internal static decimal ChantScaledAmount(CardModel? card, string key)
        {
            if (card == null) return 0m;
            if (!card.DynamicVars.TryGetValue(key, out var dynamicVar)) return 0m;

            var chantAmount = GetChantAmount(card);
            return Math.Max(0m, dynamicVar.BaseValue + chantAmount);
        }

        internal static decimal ChantScaledPowerAmount<TPower>(CardModel? card)
            where TPower : PowerModel
        {
            return ChantScaledAmount(card, typeof(TPower).Name);
        }

        private static decimal GetChantAmount(CardModel card)
        {
            return card._owner?.Creature.Powers.OfType<ChantPower>().FirstOrDefault()?.Amount ?? 0m;
        }
    }
}
