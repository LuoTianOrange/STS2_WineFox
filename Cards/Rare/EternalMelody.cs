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
    public class EternalMelody() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<ChantPower>()];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.MagicKeyword];
        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardEternalMelody,
            Const.Paths.CardEternalMelody,
            FrameMaterialPath: Const.Paths.CardEternalMelodyCosmicStarsFrameMat);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (IsUpgraded)
                await PowerCmd.Apply<EternalMelodyPlusPower>(owner, 1m, owner, this);
            else
                await PowerCmd.Apply<EternalMelodyRetentionPower>(owner, 2m, owner, this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}




