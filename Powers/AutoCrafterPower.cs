using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Cards;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class AutoCrafterPower : WineFoxPower, ICraftingCard
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.AutoCrafterPowerIcon);

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;
            if (!CraftCmd.CanCraftAny(Owner)) return;

            Flash();
            for (var i = 0; i < Amount; i++)
                await CraftCmd.CraftIntoHand(choiceContext, Owner, Owner);
        }
    }
}
