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
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEquivalentExchange);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var handCards = PileType.Hand.GetPile(owner).Cards
                .Where(c => c != this)
                .ToList();

            if (handCards.Count == 0) return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_EQUIVALENT_EXCHANGE_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 0, handCards.Count);

            var selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, handCards, owner, prefs)).ToList();
            if (selected.Count == 0) return;

            var stoneTotal = 0m; // 白卡 -> 圆石
            var ironTotal = 0m; // 蓝卡 -> 铁 
            var diamondTotal = 0m; // 金卡 -> 钻石
            var woodTotal = 0m; // 诅咒/状态/其他 -> 木板

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

            // 发放累加的资源（按 MaterialCmd 约定）
            if (stoneTotal > 0m)
                await MaterialCmd.GainMaterial<StonePower>(this, stoneTotal);
            if (ironTotal > 0m)
                await MaterialCmd.GainMaterial<IronPower>(this, ironTotal);
            if (diamondTotal > 0m)
                await MaterialCmd.GainMaterial<DiamondPower>(this, diamondTotal);
            if (woodTotal > 0m)
                await MaterialCmd.GainMaterial<WoodPower>(this, woodTotal);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
