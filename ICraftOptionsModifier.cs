using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;
using STS2_WineFox.Commands;

namespace STS2_WineFox
{
    public sealed class CraftOptionsContext
    {
        public required CombatState CombatState { get; init; }
        public required Player Owner { get; init; }
        public required Creature Crafter { get; init; }
        public Creature? Applier { get; init; }
        public CardModel? SourceCard { get; init; }
        public CraftDeliveryMode? DeliveryModeOverride { get; init; }
        public Creature? AutoPlayTarget { get; init; }
        public bool IsBonusCraft { get; init; }
        public List<CraftOption> Options { get; } = [];

        public CraftDeliveryMode ResolveDeliveryMode(CraftRecipe recipe)
        {
            return DeliveryModeOverride ?? recipe.DeliveryMode;
        }

        public CraftOption CreateOption(CraftRecipe recipe)
        {
            return new(recipe, recipe.Factory(CombatState, Owner));
        }
    }

    public interface ICraftOptionsModifier
    {
        void ModifyCraftOptions(CraftOptionsContext context);
    }
}
