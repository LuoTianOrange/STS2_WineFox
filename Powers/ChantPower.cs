using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Combat.Magic;
using STS2_WineFox.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class ChantPower : WineFoxPower, IMagicDamageModifier
    {       
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override bool AllowNegative => true;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ChantPowerIcon);

        public decimal ModifyMagicDamageAdditive(
            Creature? target,
            decimal baseAmount,
            Creature dealer,
            CardModel cardSource)
        {
            if (dealer != Owner) return 0m;
            if (!cardSource.HasModKeyword(WineFoxKeywords.Chant)) return 0m;

            return Amount;
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != Owner.Side) return;
            await PowerCmd.Remove(this);
        }
    }
}
