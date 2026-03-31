using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    public class Regroup() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(1)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRegroup);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);

            var promptLocKey = new LocString("cards", "STS2_WINE_FOX_CARD_REGROUP_CHOOSE");
            var prefs = new CardSelectorPrefs(promptLocKey, 1);
            var selected = (await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                PileType.Discard.GetPile(owner).Cards,
                owner,
                prefs)).FirstOrDefault();

            if (selected == null) return;

            await CardPileCmd.Add(selected, PileType.Hand);
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}
