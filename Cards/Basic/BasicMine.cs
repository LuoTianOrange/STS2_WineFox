using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace STS2_WineFox.Cards.Basic;

public class BasicMine() : CustomCardModel(1, CardType.Skill,
    CardRarity.Basic, TargetType.Self)
{
    public int WoodCount { get; private set; } = 2;
    public int StoneCount { get; private set; } = 2;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        
    }

    protected override void OnUpgrade()
    {
        WoodCount += 1;
        StoneCount += 1;
    }
}