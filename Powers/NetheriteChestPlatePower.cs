using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class NetheriteChestPlatePower : WineFoxPower
    {
        private bool _doubling;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.NetheriteChestPlatePowerIcon);

        public override async Task AfterBlockGained(
            Creature creature,
            decimal amount,
            ValueProp props,
            CardModel? cardSource)
        {
            if (_doubling) return;
            if (creature != Owner) return;
            if (amount <= 0m) return;

            var plating = Owner.Powers.OfType<PlatingPower>().FirstOrDefault();
            if (plating == null || plating.Amount <= 0m) return;

            _doubling = true;
            try
            {
                Flash();
                await CreatureCmd.GainBlock(Owner, amount, ValueProp.Unpowered, null);
            }
            finally
            {
                _doubling = false;
            }
        }
    }
}
