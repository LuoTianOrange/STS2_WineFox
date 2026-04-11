using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2_WineFox.Cards;
using STS2_WineFox.Commands;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class DiamondPickaxePower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.DiamondPickaxePowerIcon);

        public override Task BeforeCraftProductDelivered(CraftExecutionContext context)
        {
            if (context.Crafter != Owner) return Task.CompletedTask;
            if (context.Product == null) return Task.CompletedTask;
            if (!CraftRecipeRegistry.TryGetRecipe(context.Product.GetType(), out _)) return Task.CompletedTask;

            CardCmd.Upgrade(context.Product);
            return Task.CompletedTask;
        }
    }
}
