using MegaCrit.Sts2.Core.Combat;
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
    }

    public sealed record CraftOption(CraftRecipe Recipe, CardModel Card);

    public static class CraftRecipeRegistry
    {
        public static readonly IReadOnlyList<CraftRecipe> All =
        [
            //空手打击
            new((state, owner) => state.CreateCard<Nothing>(owner)),

            //木镐
            new((state, owner) => state.CreateCard<WoodenPickaxe>(owner),
                new CraftCost(typeof(WoodPower), 4m)
            ),
            //木剑
            new((state, owner) => state.CreateCard<WoodenSword>(owner),
                new CraftCost(typeof(WoodPower), 3m)
            ),
            //木甲
            new((state, owner) => state.CreateCard<WoodenArmor>(owner),
                new CraftCost(typeof(WoodPower), 4m)
            ),

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
            //石甲
            new((state, owner) => state.CreateCard<StoneArmor>(owner),
                new CraftCost(typeof(StonePower), 8m)
            ),

            //铁镐
            new(
                (state, owner) => state.CreateCard<IronPickaxe>(owner),
                new CraftCost(typeof(IronPower), 3m),
                new CraftCost(typeof(WoodPower), 1m)
            ),
            //铁剑
            new((state, owner) => state.CreateCard<IronSword>(owner),
                new CraftCost(typeof(IronPower), 2m),
                new CraftCost(typeof(WoodPower), 1m)
            ),
            //铁甲
            new(
                (state, owner) => state.CreateCard<IronArmor>(owner),
                new CraftCost(typeof(IronPower), 8m)
            ),

            //钻石剑
            new(
                (state, owner) => state.CreateCard<DiamondSword>(owner),
                new CraftCost(typeof(DiamondPower), 2m),
                new CraftCost(typeof(WoodPower), 1m)
            ),
            //钻石甲
            new(
                (state, owner) => state.CreateCard<DiamondArmor>(owner),
                new CraftCost(typeof(DiamondPower), 8m)
            ),
        ];
    }
}
