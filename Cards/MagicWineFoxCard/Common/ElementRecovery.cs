using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.WineFoxCard;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ElementRecovery() : WineFoxCardBase(
        0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Multiplier", 5m),
            new EnergyVar(2),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardElementRecovery);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner is not { Creature: { } creature } owner)
                return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_ELEMENT_RECOVERY_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 1);

            var selected = (await CardSelectCmd.FromHand(choiceContext, owner, prefs, null, this))
                .FirstOrDefault();

            if (selected == null)
                return;

            var cost = selected.EnergyCost.Canonical;
            var wasStatus = selected.Type == CardType.Status;

            await CardCmd.Exhaust(choiceContext, selected);
            await CreatureCmd.GainBlock(
                creature,
                cost * DynamicVars["Multiplier"].BaseValue,
                ValueProp.Move,
                play);

            if (wasStatus)
                await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Multiplier"].UpgradeValueBy(2m);
        }
    }
}
