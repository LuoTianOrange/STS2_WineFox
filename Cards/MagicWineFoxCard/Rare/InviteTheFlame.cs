using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2_WineFox.Cards.WineFoxCard;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class InviteTheFlame() : WineFoxCardBase(
        2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Burn", 2m),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<BurningPower>(),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardInviteTheFlame);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await PowerCmd.Apply<InviteTheFlamePower>(
                choiceContext,
                Owner.Creature,
                DynamicVars["Burn"].BaseValue,
                Owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
