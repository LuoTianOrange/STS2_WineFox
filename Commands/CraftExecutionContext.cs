using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;

namespace STS2_WineFox.Commands
{
    public sealed record CraftExecutionContext
    {
        public required PlayerChoiceContext ChoiceContext { get; init; }
        public required Creature Crafter { get; init; }
        public Creature? Applier { get; init; }
        public CardModel? SourceCard { get; init; }
        public required CraftRecipe Recipe { get; init; }
        public required CardModel Product { get; init; }
        public required CraftDeliveryMode DeliveryMode { get; init; }
        public Creature? AutoPlayTarget { get; init; }
        public bool IsBonusCraft { get; init; }
    }
}
