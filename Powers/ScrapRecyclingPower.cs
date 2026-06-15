using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class ScrapRecyclingPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ScrapRecyclingPowerIcon);

        public override async Task AfterCraftProductDelivered(CraftExecutionContext context)
        {
            if (context.Crafter != Owner || Amount <= 0m || context.Recipe.Costs.Length == 0)
                return;

            Flash();
            await MaterialCmd.GainMaterials(
                Owner,
                context.Recipe.Costs.Select(cost => (cost.PowerType, cost.Amount)),
                context.SourceCard,
                applyStress: false);
            await PowerCmd.ModifyAmount(this, -1m, null, context.SourceCard);
            if (Amount <= 0m)
                await PowerCmd.Remove(this);
        }
    }
}
