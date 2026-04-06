using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Commands;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public abstract class WineFoxPower : ModPowerTemplate, ICraftIntoHandListener
    {
        public virtual Task BeforeCraftIntoHand(
            PlayerChoiceContext choiceContext,
            Creature crafter,
            Creature? applier,
            CardModel? cardSource)
        {
            return Task.CompletedTask;
        }

        public virtual Task BeforeCraftProductAddToCombat(Creature crafter, CardModel product)
        {
            return Task.CompletedTask;
        }

        public virtual Task AfterCraftProductAddToCombat(Creature crafter, CardModel product)
        {
            return Task.CompletedTask;
        }

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
    }
}
