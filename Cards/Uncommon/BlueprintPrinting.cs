using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class BlueprintPrinting() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(1), new("Wood", 4m), new("Stone", 4m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBlueprintPrinting);

        protected override bool IsPlayable =>
            Owner.Creature.Powers.OfType<WoodPower>().Any(p => p.Amount >= DynamicVars["Wood"].BaseValue) &&
            Owner.Creature.Powers.OfType<StonePower>().Any(p => p.Amount >= DynamicVars["Stone"].BaseValue);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var creature = owner.Creature;

            const decimal woodCost = 4m;
            const decimal stoneCost = 4m;

            if (play.IsFirstInSeries && !MaterialCmd.IsFreePlay(play))
            {
                var hasWood = creature.Powers.OfType<WoodPower>().Any(p => p.Amount >= woodCost);
                var hasStone = creature.Powers.OfType<StonePower>().Any(p => p.Amount >= stoneCost);
                if (!hasWood || !hasStone) return;
            }

            await MaterialCmd.LoseMaterials<WoodPower, StonePower>(this, woodCost, stoneCost, play);

            var handCards = PileType.Hand.GetPile(owner).Cards
                .Where(c => c != this)
                .ToList();

            if (handCards.Count == 0) return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_BLUEPRINT_PRINTING_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 1);

            var selected = (await CardSelectCmd.FromHandForDiscard(choiceContext, owner, prefs, null, this))
                .FirstOrDefault();

            if (selected == null) return;

            var copies = DynamicVars.Cards.IntValue;
            for (var i = 0; i < copies; i++)
            {
                var clone = selected.CreateClone();

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
