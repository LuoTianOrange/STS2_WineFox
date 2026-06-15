using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.MultiMachinery
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class Reforge() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IMultiMachineryChoice
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardReforge);

        public async Task Apply(PlayerChoiceContext choiceContext, CardModel sourceCard)
        {
            var owner = sourceCard.Owner;
            var handCards = PileType.Hand.GetPile(owner).Cards
                .Where(c => !ReferenceEquals(c, sourceCard))
                .ToList();
            if (handCards.Count == 0) return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_MULTI_MACHINERY_REFORGE_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 1);
            var chosen = (await CardSelectCmd.FromHand(choiceContext, owner, prefs,
                    card => handCards.Contains(card), sourceCard))
                .FirstOrDefault();
            if (chosen == null) return;

            if (IsUpgraded)
                chosen.BaseReplayCount += 1;

            chosen.AddKeyword(CardKeyword.Exhaust);
            CardCmd.Preview(chosen);
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await Apply(choiceContext, this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
