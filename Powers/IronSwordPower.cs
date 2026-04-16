using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Token.Craft;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class IronSwordPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.IronSwordPowerIcon);

        public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
        {
            return card.Owner.Creature != Owner || card.Type is not CardType.Attack || card is IronSword
                ? playCount
                : playCount + 1;
        }

        public override async Task AfterModifyingCardPlayCount(CardModel card)
        {
            if (card is IronSword) return;
            await PowerCmd.Decrement(this);
        }
    }
}
