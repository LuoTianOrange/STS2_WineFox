using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    /// 石剑的衰减倒计时。Amount = 剩余回合数（2 → 1 → 移除）。
    /// 实际力量加/减通过游戏原生 StrengthPower 完成。
    /// </summary>
    public class StoneSwordPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => new(
            Const.Paths.StoneSwordPowerIcon,
            Const.Paths.StoneSwordPowerIcon);

        public override async Task AfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            Flash();
            // 通过原生 StrengthPower 扣减 2 点力量
            await PowerCmd.Apply<StrengthPower>(Owner, -2m, Owner, null);
            // 倒计时 -1
            await PowerCmd.ModifyAmount(this, -1m, null, null);

            if ((decimal)Amount <= 0m)
                await PowerCmd.Remove(this);
        }
    }
}