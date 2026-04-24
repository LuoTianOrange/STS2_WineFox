using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class BrushStoneFormPower : WineFoxPower
    {
        private const int CountIncrement = 2;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.BrushStoneFormPowerIcon);

        public override int DisplayAmount => Amount + GetInternalData<Data>().Amount;

        public override bool IsInstanced => true;

        protected override object InitInternalData()
        {
            return new Data
            {
                Amount = 0,
                Increment = CountIncrement,
            };
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            Flash();

            var data = GetInternalData<Data>();
            var amount = Amount + data.Amount;
            await PowerCmd.Apply<StonePower>(new ThrowingPlayerChoiceContext(), Owner, amount, Owner, null);
            data.Amount += data.Increment;
            InvokeDisplayAmountChanged();
        }

        private class Data
        {
            public int Amount;

            public int Increment;
        }
    }
}
