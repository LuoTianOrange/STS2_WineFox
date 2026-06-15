using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class SweetDream() : WineFoxCard(
        0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSweetDream);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var handCards = PileType.Hand.GetPile(owner).Cards.ToList();
            var count = handCards.Count;
            if (count == 0)
                return;

            foreach (var card in handCards)
            {
                CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Draw);
            }

            await CardPileCmd.Shuffle(choiceContext, owner);
            await CardPileCmd.Draw(choiceContext, count, owner);
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}
