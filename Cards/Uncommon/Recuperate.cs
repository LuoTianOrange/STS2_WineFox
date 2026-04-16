using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Recuperate() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(5)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRecuperate);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);

            var handPile = PileType.Hand.GetPile(owner);
            var upgraded = IsUpgraded;
            var toDiscard = handPile.Cards
                .Where(c => c.Type == CardType.Attack ||
                            (upgraded && (c.Type == CardType.Curse ||
                                          c.Type == CardType.Status ||
                                          c.Type == CardType.Quest)))
                .ToList();

            foreach (var card in toDiscard)
                await CardPileCmd.Add(card, PileType.Discard);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
