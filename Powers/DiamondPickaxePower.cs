using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class DiamondPickaxePower : WineFoxPower, ICraftOptionsModifier
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.DiamondPickaxePowerIcon);

        public void ModifyCraftOptions(CraftOptionsContext context)
        {
            if (context.Crafter != Owner) return;

            foreach (var option in context.Options)
                UpgradeCraftProduct(option.Card);
        }

        private static void UpgradeCraftProduct(CardModel card)
        {
            if (card.IsUpgraded) return;
            CardCmd.Upgrade(card, CardPreviewStyle.None);
        }
    }
}
