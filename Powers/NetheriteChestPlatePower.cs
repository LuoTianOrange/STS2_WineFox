using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class NetheriteChestPlatePower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.NetheriteChestPlatePowerIcon);

        private PlatingPower? GetPlating() => Owner.Powers.OfType<PlatingPower>().FirstOrDefault();

        public override async Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != Owner.Side) return;

            var plating = GetPlating();
            if (plating == null || plating.Amount <= 0m) return;

            Flash();
            await CreatureCmd.GainBlock(Owner, plating.Amount, ValueProp.Unpowered, null);
        }
    }
}
