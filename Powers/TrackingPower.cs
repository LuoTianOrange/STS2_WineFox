using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Tracking power applied to the player by IntermittentChanting.
    ///     Whenever the owner causes an enemy to lose HP (Unblockable damage),
    ///     the target gains 1 block and the owner gains Amount block.
    /// </summary>
    [RegisterPower]
    public class TrackingPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.TrackingPowerIcon);

        public override async Task AfterDamageReceived(
            PlayerChoiceContext choiceContext,
            Creature target,
            DamageResult result,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (dealer != Owner) return;
            if (target.Side == Owner.Side) return;
            if (result.UnblockedDamage <= 0) return;

            Flash();
            await CreatureCmd.GainBlock(target, 1m, ValueProp.Unpowered, null);
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        }
    }
}

