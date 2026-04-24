using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class IronPickaxePower : MaterialReactivePower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.IronPickaxePowerIcon);

        public override async Task AfterMaterialGain(MaterialGainEvent evt)
        {
            if (evt.Creature != Owner || Amount <= 0m)
                return;

            Flash();
            await PowerCmd.Apply<IronPower>(new ThrowingPlayerChoiceContext(), Owner, 3m, Owner, evt.SourceCard);
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -1m, null, evt.SourceCard);
            if (Amount <= 0m)
                await PowerCmd.Remove(this);
        }
    }
}
