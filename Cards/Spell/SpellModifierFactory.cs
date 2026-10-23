using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Mechanics;

namespace STS2_WineFox.Cards.Spell
{
    public static class SpellModifierFactory
    {
        private const int MaxChooseACardOptions = 3;

        public static CardModel? CreateRandom(Player owner, CardRarity maxRarity = CardRarity.Rare)
        {
            return GetDistinct(owner, 1, maxRarity).FirstOrDefault();
        }

        public static async Task<CardModel?> GrantRandomToHand(
            PlayerChoiceContext choiceContext,
            Player owner,
            CardModel? sourceCard = null,
            CardRarity maxRarity = CardRarity.Rare)
        {
            var modifier = CreateRandom(owner, maxRarity);
            if (modifier == null)
                return null;

            return await AddToHand(modifier, owner);
        }

        public static async Task<CardModel?> ChooseOneToHand(
            PlayerChoiceContext choiceContext,
            Player owner,
            int optionCount = 3,
            CardRarity maxRarity = CardRarity.Rare)
        {
            var options = GetDistinct(owner, optionCount, maxRarity);
            if (options.Count == 0)
                return null;

            if (options.Count == 1)
                return await AddToHand(options[0], owner);

            var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, owner, true);
            if (selected == null)
                return null;

            return await AddToHand(selected, owner);
        }

        /// <summary>
        ///     从随机抽取的候选里选一张，必须选出结果，只返回选择结果不入手。
        ///     <para>
        ///         选牌界面最多接受 3 张候选，超出会抛异常，因此这里做上限收敛。
        ///     </para>
        /// </summary>
        public static async Task<CardModel?> ChooseOneOfRandom(
            PlayerChoiceContext choiceContext,
            Player owner,
            CardRarity maxRarity,
            int optionCount = 3)
        {
            var options = GetDistinct(owner, Math.Min(optionCount, MaxChooseACardOptions), maxRarity);
            if (options.Count == 0)
                return null;

            return await CardSelectCmd.FromChooseACardScreen(choiceContext, options, owner);
        }

        /// <summary>
        ///     从随机抽取的候选里选一张并放入手牌，必须选出结果。
        /// </summary>
        public static async Task<CardModel?> ChooseOneOfRandomToHand(
            PlayerChoiceContext choiceContext,
            Player owner,
            CardRarity maxRarity,
            int optionCount = 3)
        {
            var selected = await ChooseOneOfRandom(choiceContext, owner, maxRarity, optionCount);
            if (selected == null)
                return null;

            return await AddToHand(selected, owner);
        }

        /// <summary>
        ///     从当前稀有度及以下的全部法术修正里选一张，使用合成页面的网格视图。
        ///     <para>
        ///         最小与最大选择数都是 1，选牌界面因此不显示确认按钮，点击卡牌即选定。
        ///     </para>
        /// </summary>
        public static async Task<CardModel?> ChooseOneFromPoolToHand(
            PlayerChoiceContext choiceContext,
            Player owner,
            CardRarity maxRarity,
            LocString? prompt = null)
        {
            var options = GetCombatPool(owner, maxRarity);
            if (options.Count == 0)
                return null;

            var prefs = new CardSelectorPrefs(
                prompt ?? new LocString("cards", "STS2_WINE_FOX_CARD_EYE_OF_PROVIDENCE_CHOOSE"),
                1, 1);

            var selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, options, owner, prefs))
                .FirstOrDefault();
            if (selected == null)
                return null;

            return await AddToHand(selected, owner);
        }

        private static List<CardModel> GetPool(Player owner, CardRarity maxRarity)
        {
            return ModelDb.CardPool<MagicWineFoxSpellModifierCardPool>()
                .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
                .Where(card => card is IMagicWineFoxSpellModifierCard)
                .Where(card => card.Rarity <= maxRarity)
                .ToList();
        }

        /// <summary>
        ///     卡池返回的是规范模型，不能直接用于战斗中的选牌与入手，
        ///     必须先转成战斗副本，否则会抛 CanonicalModelException。
        /// </summary>
        private static List<CardModel> GetCombatPool(Player owner, CardRarity maxRarity)
        {
            var pool = GetPool(owner, maxRarity);

            if (pool.Count == 0)
                return [];

            return CardFactory
                .GetDistinctForCombat(owner, pool, pool.Count, owner.RunState.Rng.CombatCardGeneration)
                .ToList();
        }

        private static List<CardModel> GetDistinct(Player owner, int count, CardRarity maxRarity)
        {
            var pool = GetPool(owner, maxRarity);

            if (pool.Count == 0)
                return [];

            return CardFactory
                .GetDistinctForCombat(owner, pool, count, owner.RunState.Rng.CombatCardGeneration)
                .ToList();
        }

        private static async Task<CardModel> AddToHand(CardModel card, Player owner)
        {
            var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, owner);
            CardCmd.PreviewCardPileAdd(result);
            return card;
        }
    }
}
