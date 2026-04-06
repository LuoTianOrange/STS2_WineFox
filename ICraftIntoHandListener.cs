using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox
{
    /// <summary>
    ///     合成入手流程的生命周期，顺序上接近 <c>CardPileCmd.AddGeneratedCardToCombat</c>。
    ///     与 <see cref="Hooks.CraftHook" /> 配合使用；参数语义与游戏内 Cmd / <c>Hook</c> 一致。
    /// </summary>
    public interface ICraftIntoHandListener
    {
        /// <summary>在选配方、扣材料之前触发（一次合成流程的起点）。</summary>
        Task BeforeCraftIntoHand(
            PlayerChoiceContext choiceContext,
            Creature crafter,
            Creature? applier,
            CardModel? cardSource);

        /// <summary>材料已消耗、产物 <paramref name="product" /> 已存在，在 <c>AddGeneratedCardToCombat</c> 之前触发。</summary>
        Task BeforeCraftProductAddToCombat(Creature crafter, CardModel product);

        /// <summary>产物已通过 <c>AddGeneratedCardToCombat</c> 加入战斗堆叠之后触发。</summary>
        Task AfterCraftProductAddToCombat(Creature crafter, CardModel product);
    }
}
