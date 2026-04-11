using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Localization;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Relics.Backpack.Effects
{
    public sealed class StonecutterBackpackEffect()
        : SophisticatedBackpackEffectBase(false)
    {
        private const string TurnCounter = "turn";

        public const string IntervalVar = "StonecutterInterval";

        public override IEnumerable<DynamicVar> CreateCanonicalVars()
        {
            return [new(IntervalVar, 0m)];
        }

        public override string BuildDescription(SophisticatedBackpack backpack)
        {
            var mainLine = BuildLine(backpack, "STS2_WINE_FOX_RELIC_SOPHISTICATED_BACKPACK_EFFECT_STONECUTTER", IntervalVar);
            var interval = Math.Max(1, backpack.DynamicVars[IntervalVar].IntValue);
            var progress = backpack.GetEffectStateInt<StonecutterBackpackEffect>(TurnCounter);
            var remaining = interval - progress;
            if (remaining <= 0)
                remaining = interval;

            var remainingLine =
                new LocString("relics", "STS2_WINE_FOX_RELIC_SOPHISTICATED_BACKPACK_REMAINING_STONECUTTER");
            remainingLine.Add("Remaining", remaining);
            return $"{mainLine}\n{remainingLine.GetFormattedText()}";
        }

        public override async Task BeforeSideTurnStart(
            SophisticatedBackpack backpack,
            PlayerChoiceContext choiceContext,
            CombatSide side,
            CombatState combatState)
        {
            if (side != backpack.Owner.Creature.Side) return;

            var progress = backpack.IncrementEffectStateInt<StonecutterBackpackEffect>(TurnCounter);
            var interval = Math.Max(1, backpack.DynamicVars[IntervalVar].IntValue);
            if (progress < interval)
            {
                backpack.RefreshDescriptionText();
                return;
            }

            var currentStone = backpack.Owner.Creature.GetPowerAmount<StonePower>();
            if (currentStone <= 0m)
            {
                backpack.SetEffectStateInt<StonecutterBackpackEffect>(TurnCounter, 0);
                backpack.RefreshDescriptionText();
                return;
            }

            backpack.SetEffectStateInt<StonecutterBackpackEffect>(TurnCounter, 0);
            backpack.RefreshDescriptionText();
            backpack.Flash();
            await PowerCmd.Apply<StonePower>(backpack.Owner.Creature, currentStone, backpack.Owner.Creature, null);
        }
    }
}
