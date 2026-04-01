using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Cards.Token;

public class GoldenSword() : WineFoxCard(
    0, CardType.Power, CardRarity.Token, TargetType.None)
{

    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGoldenSword);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var creature = Owner.Creature;
        var appliedPower = await PowerCmd.Apply<GoldenSwordPower>(creature, 1m, creature, this);
        if (appliedPower == null) return;

        if (IsUpgraded)
            appliedPower.SetNoEthereal();
        else
            appliedPower.ApplyEtherealToHand();
    }

    protected override void OnUpgrade()
    {
        
    }
}