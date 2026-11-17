using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.WineFoxCard;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class SpellOrganization() : WineFoxCardBase(
        0, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CardsVar(1),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSpellOrganization);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner is not { } owner)
                return;

            var hand = PileType.Hand.GetPile(owner);
            var candidates = hand.Cards.Where(card => card != this).ToList();

            if (candidates.Count > 0)
            {
                var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_SPELL_ORGANIZATION_CHOOSE");
                var prefs = new CardSelectorPrefs(prompt, 1);

                var selected = await CardSelectCmd.FromHand(choiceContext, owner, prefs, null, this);

                if (selected.FirstOrDefault() is { } card)
                {
                    if (card.Keywords.Contains(CardKeyword.Ethereal))
                        await CardCmd.Exhaust(choiceContext, card);
                    else
                        await CardPileCmd.Add(card, PileType.Discard);
                }
            }

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1m);
        }
    }
}
