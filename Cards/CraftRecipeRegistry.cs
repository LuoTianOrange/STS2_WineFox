using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Token;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Cards
{
    public record CraftCost(Type PowerType, decimal Amount);

    public record CraftRecipe(
        Func<CombatState, Player, CardModel> Factory,
        params CraftCost[] Costs
    )
    {
        public bool CanCraft(Creature creature)
        {
            foreach (var cost in Costs)
            {
                var power = creature.Powers.FirstOrDefault(p => p.GetType() == cost.PowerType);
                var amount = power != null ? power.Amount : 0m;
                if (amount < cost.Amount) return false;
            }

            return true;
        }

        public async Task ConsumeMaterials(CardModel card)
        {
            WineFoxActions.MaterialConsumeCountThisTurn++;
            
            var owner = card.Owner.Creature;
            foreach (var cost in Costs)
            {
                var power = owner.Powers.FirstOrDefault(p => p.GetType() == cost.PowerType);
                if (power != null)
                    await PowerCmd.ModifyAmount(power, -cost.Amount, null, card);
            }
        }
    }

    public sealed record CraftOption(CraftRecipe Recipe, CardModel Card);

    public static class CraftRecipeRegistry
    {
        public static readonly IReadOnlyList<CraftRecipe> All =
        [
            //石镐
            new(
                (state, owner) => state.CreateCard<StonePickaxe>(owner),
                new CraftCost(typeof(WoodPower), 1m),
                new CraftCost(typeof(StonePower), 3m)
            ),
            //石剑
            new(
                (state, owner) => state.CreateCard<StoneSword>(owner),
                new CraftCost(typeof(WoodPower), 1m),
                new CraftCost(typeof(StonePower), 2m)
            ),
            //铁甲
            new(
                (state, owner) => state.CreateCard<IronArmor>(owner),
                new CraftCost(typeof(IronPower), 8m)
            ),
            //木剑
            new((state, owner) => state.CreateCard<WoodenSword>(owner),
                new CraftCost(typeof(WoodPower), 3m)
            ),
            //铁镐
            new(
                (state, owner) => state.CreateCard<IronPickaxe>(owner),
                new CraftCost(typeof(IronPower), 3m),
                new CraftCost(typeof(WoodenSword), 1m)
            ),
            //钻石剑
            new(
                (state, owner) => state.CreateCard<DiamondSword>(owner),
                new CraftCost(typeof(DiamondPower), 2m),
                new CraftCost(typeof(WoodPower), 1m)
            ),
            //铁剑
            new((state, owner) => state.CreateCard<IronSword>(owner),
                new CraftCost(typeof(IronPower), 2m),
                new CraftCost(typeof(WoodPower), 1m)
            ),
            //石甲
            new((state, owner) => state.CreateCard<StoneArmor>(owner),
                new CraftCost(typeof(StonePower), 8m)
            ),
        ];

        public static IReadOnlyList<CraftOption> CreateOptions(CombatState state, Player owner)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(owner);

            return All
                .Where(recipe => recipe.CanCraft(owner.Creature))
                .Select(recipe => new CraftOption(recipe, recipe.Factory(state, owner)))
                .ToList();
        }
    }
}
