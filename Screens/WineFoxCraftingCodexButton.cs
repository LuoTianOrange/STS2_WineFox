using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.TopBar;

namespace STS2_WineFox.Screens
{
    /// <summary>
    ///     Registers the WineFox crafting-codex top-bar button through ritsulib's decoupled
    ///     <see cref="ModTopBarButtonRegistry" />. Clicking opens <see cref="NWineFoxCraftingCodexScreen" />;
    ///     the button is visible whenever:
    ///     <list type="bullet">
    ///         <item>the local character is <see cref="Character.WineFox" /> (natural owner of the system),</item>
    ///         <item>
    ///             or — per the user's scenario note — the player has somehow obtained both a WineFox
    ///             <see cref="MaterialPower" /> stack (so crafting is already yielding resources) and a
    ///             card (or power) tagged with <see cref="ICraftingCard" />.
    ///         </item>
    ///     </list>
    ///     The top-bar icon uses <see cref="Const.Paths.CraftingCodexTopBarButtonIcon" /> (sourced from
    ///     the mod’s <c>ui/button_export.png</c> asset).
    /// </summary>
    [RegisterOwnedTopBarButton("crafting_codex",
        ButtonOrder = 0,
        IconPath = Const.Paths.CraftingCodexTopBarButtonIcon)]
    public sealed class WineFoxCraftingCodexButton : IModTopBarButtonHandler
    {
        public void OnClick(ModTopBarButtonContext ctx)
        {
            // Toggle: reclicking while the codex is visible closes it (matches the vanilla deck button's
            // tap-to-close ergonomics). The codex now rides on NCardPileScreen so we can't check by type
            // — we ask NWineFoxCraftingCodexScreen whether its own synthetic pile is the current capstone.
            if (NWineFoxCraftingCodexScreen.IsCurrentlyOpen())
            {
                ctx.CloseCapstoneScreen();
                return;
            }

            NWineFoxCraftingCodexScreen.Open();
        }

        public bool IsOpen(ModTopBarButtonContext ctx)
        {
            return NWineFoxCraftingCodexScreen.IsCurrentlyOpen();
        }

        /// <summary>
        ///     Reports the total number of craftable products — i.e. the size of
        ///     <see cref="CraftRecipeRegistry.All" />. This drives the count badge so players can tell at a
        ///     glance how many recipes are registered, matching the vanilla deck button's "cards in deck"
        ///     badge idea but measured against the crafting table rather than the pile.
        /// </summary>
        public int GetCount(ModTopBarButtonContext ctx)
        {
            return CraftRecipeRegistry.All.Count;
        }

        public bool IsVisible(ModTopBarButtonContext ctx)
        {
            var player = ctx.Player;
            if (player == null)
                return false;

            if (player.Character is WineFox)
                return true;

            return HasMaterial(player) && HasCraftingCardOrPower(player);
        }

        private static bool HasMaterial(Player player)
        {
            foreach (var power in player.Creature.Powers)
                if (power is MaterialPower mp && mp.Amount > 0m)
                    return true;
            return false;
        }

        private static bool HasCraftingCardOrPower(Player player)
        {
            foreach (var power in player.Creature.Powers)
                if (power is ICraftingCard)
                    return true;

            if (HasCraftingCard(EnumerateAllCards(player)))
                return true;

            return false;
        }

        private static bool HasCraftingCard(IEnumerable<CardModel> cards)
        {
            foreach (var card in cards)
                if (card is ICraftingCard)
                    return true;
            return false;
        }

        private static IEnumerable<CardModel> EnumerateAllCards(Player player)
        {
            // In combat, the canonical deck is split across multiple card piles. Fall back to the
            // out-of-combat deck whenever the combat state hasn't been set up yet (boot, map screen, …).
            var combatState = player.PlayerCombatState;
            if (combatState != null)
                return combatState.AllCards;
            return player.Deck.Cards;
        }
    }
}
