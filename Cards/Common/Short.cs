using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class Short() : WineFoxCard(0, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Casts", 1m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardShort);

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            WineFoxKeywords.ReleaseKeyword
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var casts = (int)DynamicVars["Casts"].BaseValue;

            for (var i = 0; i < casts; i++)
                await MagicWineFoxSpellCmd.CastOldest(choiceContext, Owner, null, this);

            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Casts"].UpgradeValueBy(1m);
        }
    }
}
