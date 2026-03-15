using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Cards.Token;

public class StonePick() : CustomCardModel(0, CardType.Skill,
    CardRarity.Token, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.Digging];
    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [new PowerVar<DiggingPower>("Digging", 1m)];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CommonActions.ApplySelf<DiggingPower>(this, DynamicVars["Digging"].BaseValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Digging"].UpgradeValueBy(1m);
    }
}