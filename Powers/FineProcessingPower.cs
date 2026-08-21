using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class FineProcessingPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.FineProcessingPowerIcon);

        public override async Task BeforeCraftProductDelivered(CraftExecutionContext context)
        {
            if (context.Crafter != Owner || Amount <= 0m || context.Product.IsUpgraded)
                return;

            Flash();
            CardCmd.Upgrade(context.Product, CardPreviewStyle.None);
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -1m, null, context.SourceCard);
        }
    }
}
