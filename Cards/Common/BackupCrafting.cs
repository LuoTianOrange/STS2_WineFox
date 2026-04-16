using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class BackupCrafting() : WineFoxCard(
        0, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Craft];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBackupCrafting);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CraftCmd.CraftIntoHand(choiceContext, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Retain);
        }
    }
}
