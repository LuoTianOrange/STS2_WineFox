using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class ObjectSorting() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(3)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardObjectSorting);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);

            var handCount = PileType.Hand.GetPile(owner).Cards.Count;
            if (handCount <= 0) return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_OBJECT_SORTING_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 1);

            var selected = (await CardSelectCmd.FromHand(choiceContext, owner, prefs, null, this))
                .FirstOrDefault();

            if (selected == null) return;

            await CardPileCmd.Add(selected, PileType.Draw, CardPilePosition.Random);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1); // 3 → 4
        }
    }
}
