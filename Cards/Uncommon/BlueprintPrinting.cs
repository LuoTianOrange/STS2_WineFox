using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class BlueprintPrinting() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(2)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBlueprintPrinting);

        protected override bool IsPlayable =>
            Owner.Creature.Powers.OfType<WoodPower>().Any(p => p.Amount >= 5m) &&
            Owner.Creature.Powers.OfType<StonePower>().Any(p => p.Amount >= 5m);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var creature = owner.Creature;

            const decimal woodCost = 5m;
            const decimal stoneCost = 5m;

            var hasWood = creature.Powers.OfType<WoodPower>().Any(p => p.Amount >= woodCost);
            var hasStone = creature.Powers.OfType<StonePower>().Any(p => p.Amount >= stoneCost);

            if (!hasWood || !hasStone) return;

            await MaterialCmd.LoseMaterials<WoodPower, StonePower>(this, woodCost, stoneCost);

            var handCards = PileType.Hand.GetPile(owner).Cards
                .Where(c => c != this)
                .ToList();

            if (handCards.Count == 0) return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_BLUEPRINT_PRINTING_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 1);

            var selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, handCards, owner, prefs))
                .FirstOrDefault();

            if (selected == null) return;

            var copies = DynamicVars.Cards.IntValue;
            for (var i = 0; i < copies; i++)
            {
                var clone = selected.CreateClone();

                clone.AddKeyword(CardKeyword.Retain);
                clone.AddKeyword(CardKeyword.Exhaust);

                clone.EnergyCost.AddThisCombat(-1);

                var cardInstance = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, true);

                CardCmd.PreviewCardPileAdd(cardInstance);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1m);
        }
    }
}
