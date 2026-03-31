using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class ProductionDocumentPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ProductionDocumentPowerIcon);

        public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
        {
            if (card.Owner?.Creature != Owner) return Task.CompletedTask;

            if (!addedByPlayer && !card.IsClone) return Task.CompletedTask;

            card.AddKeyword(CardKeyword.Retain);

            return Task.CompletedTask;
        }
    }
}
