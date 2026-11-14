using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.Token;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ChannelLightning() : WineFoxCard(
        0, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        private int RequiredOverloads => IsUpgraded ? 3 : 4;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            [.. base.CanonicalKeywords, CardKeyword.Retain];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardChannelLightning);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<ThunderboltStaff>(),
            HoverTipFactory.FromPower<ChargePower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var power = MagicWineFoxSpellCmd.GetPower(Owner);
            var removed = power?.RemoveAllOverloads() ?? 0;

            if (removed < RequiredOverloads)
                return;

            await CardCmd.TransformTo<ThunderboltStaff>(this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
