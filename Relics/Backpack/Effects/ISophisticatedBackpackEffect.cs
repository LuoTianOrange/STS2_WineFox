using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using STS2_WineFox.Commands;

namespace STS2_WineFox.Relics.Backpack.Effects
{
    public interface ISophisticatedBackpackEffect
    {
        bool EnabledByDefault { get; }
        IEnumerable<DynamicVar> CreateCanonicalVars();
        string BuildDescription(SophisticatedBackpack backpack);

        decimal ModifyHandDraw(SophisticatedBackpack backpack, Player player, decimal count);

        Task BeforeSideTurnStart(
            SophisticatedBackpack backpack,
            PlayerChoiceContext choiceContext,
            CombatSide side,
            ICombatState combatState);

        Task AfterTurnEnd(
            SophisticatedBackpack backpack,
            PlayerChoiceContext choiceContext,
            CombatSide side);

        Task AfterCombatEnd(SophisticatedBackpack backpack, CombatRoom room);

        Task AfterCraftProductDelivered(SophisticatedBackpack backpack, CraftExecutionContext context);
    }
}
