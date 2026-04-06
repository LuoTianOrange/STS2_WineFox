using System;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    public class EquivalentExchange() : WineFoxCard(
        0, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Innate];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEquivalentExchange);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var handCount = PileType.Hand.GetPile(owner).Cards.Count;
            if (handCount == 0) return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_EQUIVALENT_EXCHANGE_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 0, handCount);
            var selected = (await CardSelectCmd.FromHandForDiscard(choiceContext, owner, prefs, null, this)).ToList();
            if (selected.Count == 0) return;

            var stoneTotal = 0m;
            var ironTotal = 0m;
            var diamondTotal = 0m;
            var woodTotal = 0m;

            foreach (var card in selected)
            {
                var gain = 1m;

                if (IsUpgraded && card.IsUpgraded)
                    gain *= 2m;

                switch (card.Rarity)
                {
                    case CardRarity.Common:
                        stoneTotal += gain;
                        break;
                    case CardRarity.Uncommon:
                        ironTotal += gain;
                        break;
                    case CardRarity.Rare or CardRarity.Ancient:
                        diamondTotal += gain;
                        break;
                    default:
                        woodTotal += gain;
                        break;
                }

                await CardPileCmd.Add(card, PileType.Exhaust);
            }

            var gains = new List<(Type Type, decimal Amount)>(4);
            if (stoneTotal > 0m)
                gains.Add((typeof(StonePower), stoneTotal));
            if (ironTotal > 0m)
                gains.Add((typeof(IronPower), ironTotal));
            if (diamondTotal > 0m)
                gains.Add((typeof(DiamondPower), diamondTotal));
            if (woodTotal > 0m)
                gains.Add((typeof(WoodPower), woodTotal));

            if (gains.Count > 0)
                await MaterialCmd.GainMaterials(this, gains);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
