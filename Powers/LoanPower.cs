using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class LoanPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Debuff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.LoanPowerIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(0)];
        
        private bool _applied;
        
        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (_applied || player != Owner.Player || AmountOnTurnStart == 0)
                return count;

            _applied = true;
            return Math.Max(0m, count - Amount);
        }

        public override Task AfterModifyingHandDraw()
        {
            if (_applied)
                Flash();
            return Task.CompletedTask;
        }
        
        public override async Task AfterSideTurnStart(
            CombatSide side,
            IReadOnlyList<Creature> participants,
            ICombatState combatState)
        {
            if (participants.Contains(Owner) && AmountOnTurnStart != 0)
                await PowerCmd.Remove(this);
        }
    }
}
