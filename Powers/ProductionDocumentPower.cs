using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class ProductionDocumentPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ProductionDocumentPowerIcon);


        public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
        {
            if (card.Owner?.Creature != Owner) return Task.CompletedTask;
            if (creator == null) return Task.CompletedTask;

            card.AddKeyword(CardKeyword.Retain);
            return Task.CompletedTask;
        }

        public override async Task AfterCardRetained(CardModel card)
        {
            if (card.Owner?.Creature != Owner) return;

            Flash();
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        }
    }
}
