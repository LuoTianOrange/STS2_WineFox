using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class AutomaticLoading() : WineFoxCard(
        2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAutomaticLoading);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<AutomaticLoadingPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner?.Creature is not { } owner)
                return;

            await PowerCmd.Apply<AutomaticLoadingPower>(
                choiceContext,
                owner,
                1m,
                owner,
                this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
