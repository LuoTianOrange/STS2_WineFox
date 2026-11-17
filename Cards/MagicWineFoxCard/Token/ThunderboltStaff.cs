using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2_WineFox.Cards.WineFoxCard;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxTokenCardPool))]
    public class ThunderboltStaff() : WineFoxCardBase(
        1, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardThunderboltStaff);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ThunderboltStaffPower>(),
            HoverTipFactory.FromPower<ChargePower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await PowerCmd.Apply<ThunderboltStaffPower>(
                choiceContext, Owner.Creature, IsUpgraded ? 15m : 10m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
