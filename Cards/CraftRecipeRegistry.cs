using System.Diagnostics.CodeAnalysis;
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
        Type ProductCardType,
        Func<CombatState, Player, CardModel> Factory,
        params CraftCost[] Costs
    )
    {
        public bool CanCraft(Creature creature)
        {
            return !(from cost in Costs
                let power = creature.Powers.FirstOrDefault(p => p.GetType() == cost.PowerType)
                let amount = power?.Amount ?? 0m
                where amount < cost.Amount
                select cost).Any();
        }
    }

    public sealed record CraftOption(CraftRecipe Recipe, CardModel Card);

    public static class CraftRecipeRegistry
    {
        public static readonly IReadOnlyList<CraftRecipe> All =
        [
            //空手打击
            new(typeof(Nothing), (state, owner) => state.CreateCard<Nothing>(owner)),

            //木镐
            new(typeof(WoodenPickaxe), (state, owner) => state.CreateCard<WoodenPickaxe>(owner),
                new CraftCost(typeof(WoodPower), 4m)
            ),
            //木剑
            new(typeof(WoodenSword), (state, owner) => state.CreateCard<WoodenSword>(owner),
                new CraftCost(typeof(WoodPower), 3m)
            ),
            //木甲
            new(typeof(WoodenArmor), (state, owner) => state.CreateCard<WoodenArmor>(owner),
                new CraftCost(typeof(WoodPower), 4m)
            ),

            //石镐
            new(
                typeof(StonePickaxe),
                (state, owner) => state.CreateCard<StonePickaxe>(owner),
                new CraftCost(typeof(WoodPower), 1m),
                new CraftCost(typeof(StonePower), 3m)
            ),
            //石剑
            new(
                typeof(StoneSword),
                (state, owner) => state.CreateCard<StoneSword>(owner),
                new CraftCost(typeof(WoodPower), 1m),
                new CraftCost(typeof(StonePower), 2m)
            ),
            //石甲
            new(typeof(StoneArmor), (state, owner) => state.CreateCard<StoneArmor>(owner),
                new CraftCost(typeof(StonePower), 8m)
            ),

            //铁镐
            new(
                typeof(IronPickaxe),
                (state, owner) => state.CreateCard<IronPickaxe>(owner),
                new CraftCost(typeof(IronPower), 3m),
                new CraftCost(typeof(WoodPower), 1m)
            ),
            //铁剑
            new(typeof(IronSword), (state, owner) => state.CreateCard<IronSword>(owner),
                new CraftCost(typeof(IronPower), 2m),
                new CraftCost(typeof(WoodPower), 1m)
            ),
            //铁甲
            new(
                typeof(IronArmor),
                (state, owner) => state.CreateCard<IronArmor>(owner),
                new CraftCost(typeof(IronPower), 8m)
            ),
            
            //钻石镐
            new(
                typeof(DiamondPickaxe),
                (state, owner) => state.CreateCard<DiamondPickaxe>(owner),
                new CraftCost(typeof(DiamondPower), 3m),
                new CraftCost(typeof(WoodPower), 1m)
            ),
            //钻石剑
            new(
                typeof(DiamondSword),
                (state, owner) => state.CreateCard<DiamondSword>(owner),
                new CraftCost(typeof(DiamondPower), 2m),
                new CraftCost(typeof(WoodPower), 1m)
            ),
            //钻石甲
            new(
                typeof(DiamondArmor),
                (state, owner) => state.CreateCard<DiamondArmor>(owner),
                new CraftCost(typeof(DiamondPower), 8m)
            ),
            //盾牌
            new(
                typeof(Shield),
                (state, owner) => state.CreateCard<Shield>(owner),
                new CraftCost(typeof(WoodPower), 6m),
                new CraftCost(typeof(IronPower), 1m)
            ),
        ];

        private static readonly Dictionary<Type, CraftRecipe> ByProductType = BuildByProductType();

        private static Dictionary<Type, CraftRecipe> BuildByProductType()
        {
            var map = new Dictionary<Type, CraftRecipe>();
            foreach (var recipe in All)
                map[recipe.ProductCardType] = recipe;
            return map;
        }

        public static bool TryGetRecipe(Type cardType, [NotNullWhen(true)] out CraftRecipe? recipe)
        {
            return ByProductType.TryGetValue(cardType, out recipe);
        }
    }
}
