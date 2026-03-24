using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public abstract class WineFoxPower : ModPowerTemplate
    {
        protected static PowerAssetProfile Icons(string iconPath, string? bigIconPath = null)
            => new(iconPath, bigIconPath ?? iconPath);

        // sealed：确保每个回合开始时必定重置计数器，再调用子类逻辑
        public sealed override async Task AfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            WineFoxActions.MaterialConsumeCountThisTurn = 0;
            await OnAfterPlayerTurnStart(choiceContext, player);
        }

        protected virtual Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
            => Task.CompletedTask;
    }
}