using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
namespace STS2_WineFox;

/// <summary>
///     合成入手流程的生命周期，顺序上接近 <c>CardPileCmd.AddGeneratedCardToCombat</c>：
///     开始合成 → 产物实例已确定且材料已扣 → 加入堆叠前 → 加入堆叠后（含游戏的 AfterCardGeneratedForCombat）。
/// </summary>
public interface ICraftIntoHandListener
{
    /// <summary>在选配方、扣材料之前触发（一次合成流程的起点）。</summary>
    Task BeforeCraftIntoHandStart(PlayerChoiceContext choiceContext, Player owner, CardModel source);

    /// <summary>材料已消耗、产物 <paramref name="product" /> 已存在，在 <c>AddGeneratedCardToCombat</c> 之前触发。</summary>
    Task BeforeCraftProductAddToCombat(Player owner, CardModel product);

    /// <summary>产物已通过 <c>AddGeneratedCardToCombat</c> 加入战斗堆叠之后触发。</summary>
    Task AfterCraftProductAddToCombat(Player owner, CardModel product);
}
