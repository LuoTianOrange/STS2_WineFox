using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class WorkbenchBackpack() : WineFoxCard(
        2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new EnergyVar(1)];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Craft];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardWorkbenchBackpack);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);

            await CraftCmd.Craft(choiceContext, Owner.Creature, Owner.Creature, this);

            EnergyCost.AddThisCombat(-1);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Energy.UpgradeValueBy(1m);
        }
    }
}
