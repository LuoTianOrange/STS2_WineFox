using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Relics.Backpack.Effects
{
    public sealed class SmeltingBackpackEffect()
        : SophisticatedBackpackEffectBase(false)
    {
        public const string IronVar = "SmeltingIronAmount";

        public override IEnumerable<DynamicVar> CreateCanonicalVars()
        {
            return [new(IronVar, 0m)];
        }

        public override string BuildDescription(SophisticatedBackpack backpack)
        {
            return BuildLine(backpack, "STS2_WINE_FOX_RELIC_SOPHISTICATED_BACKPACK_EFFECT_SMELTING", IronVar);
        }

        public override async Task BeforeSideTurnStart(
            SophisticatedBackpack backpack,
            PlayerChoiceContext choiceContext,
            CombatSide side,
            CombatState combatState)
        {
            if (side != backpack.Owner.Creature.Side) return;

            backpack.NotifyBackpackEffectTriggered();
            await PowerCmd.Apply<IronPower>(
                backpack.Owner.Creature,
                backpack.DynamicVars[IronVar].BaseValue,
                backpack.Owner.Creature,
                null);
        }
    }
}
