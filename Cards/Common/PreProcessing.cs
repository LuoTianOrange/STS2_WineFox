using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common;

public class PreProcessing() : WineFoxCard(
    1, CardType.Attack, CardRarity.Common, TargetType.Self)
{public override bool GainsBlock => true;
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(0, ValueProp.Move),new ("Wood", 3m), new("Stone", 3m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate,CardKeyword.Exhaust];
    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPreProcessing);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await MaterialCmd.GainMaterial<WoodPower>(this, DynamicVars["Wood"].BaseValue);
        await MaterialCmd.GainMaterial<StonePower>(this, DynamicVars["Stone"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}