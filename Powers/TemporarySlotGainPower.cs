using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class TemporarySlotGainPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.TemporarySlotGainPowerIcon);

        public override Task AfterRemoved(Creature oldOwner)
        {
            if (oldOwner.Player is { } player)
                MagicWineFoxSpellCmd.GetPower(player)
                    ?.ClearSlotCapacityOffset(MagicWineFoxSpellSlotPower.TemporarySlotGainSource);

            return Task.CompletedTask;
        }

        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (side != Owner.Side)
                return;

            await PowerCmd.Remove(this);
        }
    }
}
