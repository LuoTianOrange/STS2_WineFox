using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class MidasTouchPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.MidasTouchPowerIcon);

        public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
        {
            if (card.Owner?.Creature != Owner)
                return Task.CompletedTask;

            if (card.IsUpgraded || !card.IsUpgradable)
                return Task.CompletedTask;

            CardCmd.Upgrade(card, CardPreviewStyle.None);
            return Task.CompletedTask;
        }
    }
}
