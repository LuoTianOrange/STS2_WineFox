using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon;

public class ProductionDocument() : WineFoxCard(
    1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardProductionDocument);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var owner = Owner;
        
        await PowerCmd.Apply<ProductionDocumentPower>(
            owner.Creature,
            1m,
            owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}