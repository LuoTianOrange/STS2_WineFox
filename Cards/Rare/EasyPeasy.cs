using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare;

public class EasyPeasy() : WineFoxCard(
    0, CardType.Power, CardRarity.Rare, TargetType.None)
{

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEasyPeasy);

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new ("EasyPeasy", 1m), new ("RadiationLeak", 1m),new EnergyVar(1)];
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        DynamicVars.Energy.BaseValue = DynamicVars["EasyPeasy"].BaseValue;
        
        await PowerCmd.Apply<EasyPeasyPower>(
            Owner.Creature,  DynamicVars["EasyPeasy"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<RadiationLeakPower>(
            Owner.Creature, DynamicVars["RadiationLeak"].BaseValue, Owner.Creature, this);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars["EasyPeasy"].UpgradeValueBy(1m);
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}