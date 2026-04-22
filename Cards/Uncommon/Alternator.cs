using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Alternator() : WineFoxCard(
        0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new EnergyVar(1), new EnergyVar("BonusEnergy", 1)];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stress];
        
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAlternator);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);

            var stressPower = Owner.Creature.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);
            if (stressPower != null)
            {
                await PowerCmd.ModifyAmount(stressPower, -1m, null, this);
                await PlayerCmd.GainEnergy(DynamicVars["BonusEnergy"].BaseValue, Owner);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["BonusEnergy"].UpgradeValueBy(1);
        }
    }
}
