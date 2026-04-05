using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare;

public class AutoCrafter() : WineFoxCard(
    1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    
    protected override IEnumerable<string> RegisteredKeywordIds =>
        [WineFoxKeywords.Craft];
    public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAutoCrafter);

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        var creature = Owner.Creature;
        var target = play.Target ?? creature;
        
        await PowerCmd.Apply<AutoCrafterPower>(target, 1m, creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
}