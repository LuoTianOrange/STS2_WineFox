using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class ShardCopyPower : WineFoxPower
    {
        private bool _reacting;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ShardCopyPowerIcon);

        public override async Task AfterPowerAmountChanged(
            PowerModel power,
            decimal amount,
            Creature? applier,
            CardModel? cardSource)
        {
            if (_reacting) return;
            if (power.Owner != Owner) return;
            if (power is not ChantPower) return;
            if (amount <= 0m) return;

            var extraChant = Amount;
            if (extraChant <= 0m) return;

            _reacting = true;
            try
            {
                Flash();
                await PowerCmd.Apply<ChantPower>(Owner, extraChant, Owner, cardSource);
            }
            finally
            {
                _reacting = false;
            }
        }
    }
}
