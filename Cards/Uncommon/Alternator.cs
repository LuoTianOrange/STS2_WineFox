using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon;

public class Alternator() : WineFoxCard(
    1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new ("Stress",1),new EnergyVar(2)];

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAlternator);

    protected override bool IsPlayable =>
        Owner.Creature.Powers.OfType<StressPower>().Any(p => (decimal)p.Amount > 0);
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var energyToGain = (decimal)DynamicVars.Energy.IntValue;
        
        await PowerCmd.Apply<StressPower>(Owner.Creature, -1, Owner.Creature, this);
        
        await PlayerCmd.GainEnergy(energyToGain, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}