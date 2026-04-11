using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace STS2_WineFox.Relics.Backpack.Effects
{
    public sealed class FeedingBackpackEffect()
        : SophisticatedBackpackEffectBase(false)
    {
        public const string RegenVar = "FeedingRegenAmount";

        public override IEnumerable<DynamicVar> CreateCanonicalVars()
        {
            return [new(RegenVar, 0m)];
        }

        public override string BuildDescription(SophisticatedBackpack backpack)
        {
            return BuildLine(backpack, "STS2_WINE_FOX_RELIC_SOPHISTICATED_BACKPACK_EFFECT_FEEDING", RegenVar);
        }

        public override async Task BeforeSideTurnStart(
            SophisticatedBackpack backpack,
            PlayerChoiceContext choiceContext,
            CombatSide side,
            CombatState combatState)
        {
            if (side != backpack.Owner.Creature.Side) return;

            backpack.Flash();
            await PowerCmd.Apply<RegenPower>(
                backpack.Owner.Creature,
                backpack.DynamicVars[RegenVar].BaseValue,
                backpack.Owner.Creature,
                null);
        }
    }
}
