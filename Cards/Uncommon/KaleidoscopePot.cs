using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class KaleidoscopePot() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(1),new("Transform",1)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardKaleidoscopePot);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);

            var toSelect = (int)DynamicVars["Transform"].BaseValue;
            if (toSelect <= 0) return;

            var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, toSelect);

            var selected = (await CardSelectCmd.FromHand(choiceContext, owner, prefs, null, this)).ToList();
            if (selected.Count == 0) return;

            foreach (var original in selected)
                await CardCmd.TransformToRandom(original, owner.RunState.Rng.CombatCardSelection);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
            DynamicVars["Transform"].UpgradeValueBy(1);
        }
    }
}

