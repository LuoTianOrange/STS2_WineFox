using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class BrushStoneFormPower : WineFoxPower
    {
        private const int Increment = 2;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.BrushStoneFormPowerIcon);

        public override int DisplayAmount => GetInternalData<Data>().Amount;

        public override bool IsInstanced => true;

        protected override object InitInternalData()
        {
            return new Data();
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            Flash();

            var data = GetInternalData<Data>();
            await PowerCmd.Apply<StonePower>(Owner, Amount, Owner, null);
            data.Amount += Increment;
            InvokeDisplayAmountChanged();
        }

        private class Data
        {
            public int Amount;

            public int Increment;
        }
    }
}
