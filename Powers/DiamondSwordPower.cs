using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class DiamondSwordPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.DiamondSwordPowerIcon);

        public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
        {
            return card.Owner.Creature != Owner
                   || card.Type is not CardType.Attack
                   || CombatManager.Instance.History.CardPlaysStarted.Count(e =>
                       e.Actor == Owner
                       && e.CardPlay.Card.Type is CardType.Attack
                       && e.CardPlay.IsFirstInSeries
                       && e.HappenedThisTurn(CombatState)) >= Amount
                ? playCount
                : playCount + 1;
        }
    }
}
