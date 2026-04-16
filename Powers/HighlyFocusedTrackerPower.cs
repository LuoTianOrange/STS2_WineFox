using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class HighlyFocusedTrackerPower : WineFoxPower
    {
        private bool _reacting;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.HighlyFocusedPowerIcon);

        public async Task ApplyInitialBonus(decimal strength)
        {
            _reacting = true;
            try
            {
                await PowerCmd.Apply<HighlyFocusedPower>(Owner, strength, Owner, null);
            }
            finally
            {
                _reacting = false;
            }
        }

        public override async Task AfterPowerAmountChanged(
            PowerModel power,
            decimal amount,
            Creature? applier,
            CardModel? cardSource)
        {
            if (_reacting) return;
            if (power.Owner != Owner) return;
            if (power is not StrengthPower) return;
            if (amount <= 0m) return;

            _reacting = true;
            try
            {
                Flash();
                await PowerCmd.Apply<HighlyFocusedPower>(Owner, amount, Owner, null);
            }
            finally
            {
                _reacting = false;
            }
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == Owner.Side)
                await PowerCmd.Remove(this);
        }
    }
}
