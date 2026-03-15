using STS2_WineFox.Cards.Token;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Cards;

// 一个合成配方
public record CraftRecipe(
    string Name,
    Dictionary<Type, int> Requirements, // PowerType → 所需数量
    Type ResultCardType                 // 合成后加入手牌的卡牌类型
)
{
    // 判断当前材料是否满足合成条件
    public bool CanCraft(/* AbstractCreature player */)
    {
        foreach (var (powerType, required) in Requirements)
        {
            // TODO: 根据 BaseLib API 获取 player 对应 power 的 stacks
            // int current = player.GetPower(powerType)?.Amount ?? 0;
            // if (current < required) return false;
        }
        return true;
    }

    // 消耗材料
    public async Task ConsumeMaterials(/* AbstractCreature player */)
    {
        foreach (var (powerType, amount) in Requirements)
        {
            // TODO: await Actions.ReducePower(player, powerType, amount);
        }
    }
}

// 全局配方注册表（静态）
public static class CraftRecipeRegistry
{
    public static readonly List<CraftRecipe> All =
    [
        // 石镐：1块木板(=2木棍) + 3圆石
        new CraftRecipe(
            Name: "石镐",
            Requirements: new() {
                { typeof(WoodPower), 2 },
                { typeof(StonePower), 3 }
            },
            ResultCardType: typeof(StonePick) // 后续实现
        ),
    ];
}