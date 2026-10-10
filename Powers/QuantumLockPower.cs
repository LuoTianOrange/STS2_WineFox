using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     量子锁定：本回合你的法术无法被格挡。
    /// </summary>
    [RegisterPower]
    public class QuantumLockPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.QuantumLockPowerIcon);

        /// <summary>本回合的法术伤害是否无视格挡。</summary>
        public bool IgnoresBlock { get; private set; }

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            IgnoresBlock = true;
            return Task.CompletedTask;
        }

        /// <summary>己方回合结束即失效。</summary>
        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (side != Owner.Side)
                return;

            IgnoresBlock = false;
            await PowerCmd.Remove(this);
        }

        /// <summary>查询该玩家本回合的法术是否无视格挡。</summary>
        public static bool IsActiveFor(Creature? creature)
        {
            return creature?.Powers.OfType<QuantumLockPower>()
                .Any(power => power.IgnoresBlock) == true;
        }
    }
}
