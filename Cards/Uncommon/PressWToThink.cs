using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon;

public class PressWToThink() : WineFoxCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPressWToThink);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [ new IntVar("Replay", 1m),new IntVar("Exhaust",1m) ];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var owner = Owner;

        var drawPileCards = PileType.Draw.GetPile(owner).Cards;
        var topCards = drawPileCards.Take(3).ToList();

        if (topCards.Count == 0) return;
        
        var promptLocKey = new LocString("cards", "STS2_WINE_FOX_CARD_PRESS_W_TO_THINK_CHOOSE");
        var prefs = new CardSelectorPrefs(promptLocKey, 1);
        
        IReadOnlyList<CardModel> options = topCards;
        var selectedList = await CardSelectCmd.FromSimpleGrid(choiceContext, options, owner, prefs);
        var chosen = selectedList.FirstOrDefault();
        if (chosen == null)
            return;

        chosen.BaseReplayCount += DynamicVars["Replay"].IntValue;
        chosen.AddKeyword(CardKeyword.Exhaust);
        CardCmd.Preview(chosen);
        await CardPileCmd.Add(chosen, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}