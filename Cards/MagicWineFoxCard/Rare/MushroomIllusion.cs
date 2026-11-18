using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.WineFoxCard;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class MushroomIllusion() : WineFoxCardBase(
        1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
            CardKeyword.Exhaust,
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMushroomIllusion);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner is not { } owner)
                return;

            var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1);
            var selected = (await CardSelectCmd.FromHand(choiceContext, owner, prefs, IsSelectable, this))
                .FirstOrDefault();

            if (selected == null)
                return;

            var deckVersion = selected.DeckVersion;
            var result = await CardCmd.TransformToRandom(selected, owner.PlayerRng.Transformations);

            if (deckVersion == null || deckVersion == selected || result.cardAdded == null)
                return;

            var deckReplacement = deckVersion.CardScope.CreateCard(result.cardAdded.CanonicalInstance, owner);
            await CardCmd.Transform(deckVersion, deckReplacement);
        }

        private bool IsSelectable(CardModel card) => card != this && card.IsTransformable;

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
