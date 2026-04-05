using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class DiamondPickaxePower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.DiamondPickaxePowerIcon);

        public override Task BeforeCraftProductAddToCombat(Player owner, CardModel product)
        {
            if (owner.Creature != Owner) return Task.CompletedTask;
            if (!CraftRecipeRegistry.TryGetRecipe(product.GetType(), out _)) return Task.CompletedTask;

            CardCmd.Upgrade(product);
            return Task.CompletedTask;
        }
    }
}
