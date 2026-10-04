using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Mechanics;

namespace STS2_WineFox.Cards.Spell
{
    public static class SpellModifierFactory
    {
        public static CardModel? CreateRandom(Player owner)
        {
            return GetDistinct(owner, 1).FirstOrDefault();
        }

        public static async Task<CardModel?> GrantRandomToHand(
            PlayerChoiceContext choiceContext,
            Player owner,
            CardModel? sourceCard = null)
        {
            var modifier = CreateRandom(owner);
            if (modifier == null)
                return null;

            return await AddToHand(modifier, owner);
        }

        public static async Task<CardModel?> ChooseOneToHand(
            PlayerChoiceContext choiceContext,
            Player owner,
            int optionCount = 3)
        {
            var options = GetDistinct(owner, optionCount);
            if (options.Count == 0)
                return null;

            if (options.Count == 1)
                return await AddToHand(options[0], owner);

            var selected = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, owner, true);
            if (selected == null)
                return null;

            return await AddToHand(selected, owner);
        }

        private static List<CardModel> GetDistinct(Player owner, int count)
        {
            var pool = ModelDb.CardPool<MagicWineFoxTokenCardPool>()
                .GetUnlockedCards(owner.UnlockState, owner.RunState.CardMultiplayerConstraint)
                .Where(card => card is IMagicWineFoxSpellModifierCard)
                .ToList();

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
