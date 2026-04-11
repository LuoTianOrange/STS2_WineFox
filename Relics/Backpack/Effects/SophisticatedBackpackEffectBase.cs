using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using STS2_WineFox.Commands;

namespace STS2_WineFox.Relics.Backpack.Effects
{
    public abstract class SophisticatedBackpackEffectBase(bool enabledByDefault)
        : ISophisticatedBackpackEffect
    {
        public bool EnabledByDefault { get; } = enabledByDefault;

        public abstract IEnumerable<DynamicVar> CreateCanonicalVars();
        public abstract string BuildDescription(SophisticatedBackpack backpack);

        public virtual decimal ModifyHandDraw(SophisticatedBackpack backpack, Player player, decimal count)
        {
            return count;
        }

        public virtual Task BeforeSideTurnStart(
            SophisticatedBackpack backpack,
            PlayerChoiceContext choiceContext,
            CombatSide side,
            CombatState combatState)
        {
            return Task.CompletedTask;
        }

        public virtual Task AfterTurnEnd(
            SophisticatedBackpack backpack,
            PlayerChoiceContext choiceContext,
            CombatSide side)
        {
            return Task.CompletedTask;
        }

        public virtual Task AfterCombatEnd(SophisticatedBackpack backpack, CombatRoom room)
        {
            return Task.CompletedTask;
        }

        public virtual Task AfterCraftProductDelivered(SophisticatedBackpack backpack, CraftExecutionContext context)
        {
            return Task.CompletedTask;
        }

        protected static string BuildLine(SophisticatedBackpack backpack, string locKey, params string[] varNames)
        {
            var line = new LocString("relics", locKey);
            foreach (var varName in varNames)
                line.Add(backpack.DynamicVars[varName]);
            return line.GetFormattedText();
        }
    }
}
