using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox;
using STS2_WineFox.Commands;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public abstract class WineFoxPower : ModPowerTemplate, ICraftIntoHandListener
    {
        protected static PowerAssetProfile Icons(string iconPath, string? bigIconPath = null)
        {
            return new(iconPath, bigIconPath ?? iconPath);
        }

        public sealed override Task AfterPlayerTurnStartEarly(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature == Owner)
                CraftCmd.ObserveTurnStarted(choiceContext, player);

            return Task.CompletedTask;
        }

        public sealed override async Task AfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            await OnAfterPlayerTurnStart(choiceContext, player);
        }

        protected virtual Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            return Task.CompletedTask;
        }

        public virtual Task BeforeCraftIntoHandStart(
            PlayerChoiceContext choiceContext, Player owner, CardModel source) =>
            Task.CompletedTask;

        public virtual Task BeforeCraftProductAddToCombat(Player owner, CardModel product) =>
            Task.CompletedTask;

        public virtual Task AfterCraftProductAddToCombat(Player owner, CardModel product) =>
            Task.CompletedTask;
    }
}
