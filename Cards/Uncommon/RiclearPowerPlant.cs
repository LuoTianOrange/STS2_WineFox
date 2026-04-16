using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class RiclearPowerPlant() : WineFoxCard(
        2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new BlockVar(13, ValueProp.Move), new("Stress", 2m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRiclearPowerPlant);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            await PowerCmd.Apply<StressPower>(
                Owner.Creature, DynamicVars["Stress"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Block"].UpgradeValueBy(3m);
        }
    }
}
