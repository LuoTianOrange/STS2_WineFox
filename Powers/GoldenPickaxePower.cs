using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class GoldenPickaxePower : MaterialReactivePower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.GoldenPickaxePowerIcon);

        public override async Task AfterMaterialGain(MaterialGainEvent evt)
        {
            var totalAmount = evt.TotalAmount;
            if (evt.Creature != Owner || totalAmount <= 0m) return;

            Flash();

            await CreatureCmd.GainBlock(Owner, totalAmount, ValueProp.Unpowered, null);

            var combatState = Owner.CombatState;
            if (combatState == null) return;

            var enemy = combatState.Enemies
                .Where(e => e.IsAlive)
                .MinBy(_ => Random.Shared.Next());
            if (enemy == null) return;

            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                enemy,
                totalAmount * Amount,
                ValueProp.Move,
                Owner,
                evt.SourceCard);
        }
    }
}
