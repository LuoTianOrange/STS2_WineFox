using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Cards.Token.MultiMachinery
{
    public interface IMultiMachineryChoice
    {
        Task Apply(PlayerChoiceContext choiceContext, CardModel sourceCard);
    }
}
