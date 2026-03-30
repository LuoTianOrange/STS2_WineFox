using System;
using System.Linq;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common;

public class ChangeEquipment() : WineFoxCard(
    0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [ new CardsVar(2) ];

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardChangeEquipment);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var owner = Owner;

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);

        var handCards = PileType.Hand.GetPile(owner).Cards;
        var toSelect = Math.Min(2, handCards.Count);
        if (toSelect <= 0) return;

        var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_CHANGE_EQUIPMENT_CHOOSE");
        var prefs = new CardSelectorPrefs(prompt, toSelect);

        var selected = (await CardSelectCmd.FromSimpleGrid(choiceContext, handCards, owner, prefs)).ToList();
        if (selected.Count == 0) return;

        foreach (var card in selected)
            await CardPileCmd.Add(card, PileType.Draw);

        await CardPileCmd.Shuffle(choiceContext, owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}