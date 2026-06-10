using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class EnderChest() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Cards", 1m), new EnergyVar(1)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEnderChest);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var discardPile = PileType.Discard.GetPile(owner);

            if (discardPile.Cards.Count <= 0) return;

            var toSelect = Math.Min((int)DynamicVars["Cards"].BaseValue, discardPile.Cards.Count);
            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_ENDER_CHEST_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, toSelect);

            var selected = await CardSelectCmd.FromSimpleGrid(
                choiceContext,
                discardPile.Cards,
                owner,
                prefs);

            foreach (var card in selected)
            {
                await CardPileCmd.Add(card, PileType.Hand);
                card.AddKeyword(CardKeyword.Retain);
            }

            PlayerCmd.EndTurn(owner, false);

            await PowerCmd.Apply<EnergyNextTurnPower>(
                new ThrowingPlayerChoiceContext(),
                owner.Creature,
                1,
                owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Cards"].UpgradeValueBy(1m); // 1 → 2
        }
    }
}
