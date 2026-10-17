using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ElementRecovery() : WineFoxCard(
        0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Multiplier", 4m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardElementRecovery);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var owner = Owner;
            if (owner?.Creature is not { } creature)
                return;

            if (PileType.Hand.GetPile(owner).Cards.Count(c => c != this) == 0)
                return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_ELEMENT_RECOVERY_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 0, 1);

            var selected = (await CardSelectCmd.FromHand(choiceContext, owner, prefs, null, this))
                .FirstOrDefault();
            if (selected == null)
                return;

            var cost = selected.EnergyCost.Canonical;
            await CardCmd.Exhaust(choiceContext, selected);
            await CreatureCmd.GainBlock(
                creature,
                cost * DynamicVars["Multiplier"].BaseValue,
                ValueProp.Move,
                play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Multiplier"].UpgradeValueBy(1m);
        }
    }
}
