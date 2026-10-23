using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class NothingWastedPower : MaterialReactivePower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.NothingWastedPowerIcon);

        public override async Task AfterMaterialConsume(MaterialConsumeEvent evt)
        {
            if (evt.Creature != Owner || evt.TotalAmount <= 0m)
                return;

            Flash();
            await CreatureCmd.GainBlock(Owner, evt.TotalAmount * Amount, ValueProp.Unpowered, null);
        }
    }
}
