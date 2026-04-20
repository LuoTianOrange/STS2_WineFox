using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Applied by OtherworldCrossing (异界跨越).
    ///     At the start of your turn, for each card currently in hand,
    ///     adds an Ethereal + Exhaust copy of that card to your hand.
    ///     Amount >= 2 (upgraded): copies also gain Innate.
    /// </summary>
    [RegisterPower]
    public class OtherworldCrossingPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.None;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.OtherworldCrossingPowerIcon);

        public override bool IsInstanced => true;
        
        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            if (PileType.Hand.GetPile(player).Cards.Count == 0) return;

            Flash();
            
            var prompt = new LocString("cards", "STS2_WINE_FOX_POWER_OTHERWORLD_CROSSING_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 1);

            var selected = (await CardSelectCmd.FromHandForDiscard(choiceContext, player, prefs, null, null))
                .FirstOrDefault();

            if (selected == null) return;

            var clone = selected.CreateClone();
            clone.AddKeyword(CardKeyword.Ethereal);
            clone.AddKeyword(CardKeyword.Exhaust);

            var cardInstance = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, true);
            CardCmd.PreviewCardPileAdd(cardInstance);
        }
    }
}

