using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class MagicOverloaded() : WineFoxCard(
        1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicOverloaded);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ChantPower>(),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.MagicKeyword];
        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            var extraRate = IsUpgraded ? 75m : 50m;
            await PowerCmd.Apply<MagicOverloadedPower>(owner, extraRate, owner, this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
