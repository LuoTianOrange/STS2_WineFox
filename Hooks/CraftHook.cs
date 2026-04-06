using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Hooks
{
    /// <summary>
    ///     合成流程钩子。遍历与分发方式对齐 <see cref="MegaCrit.Sts2.Core.Hooks.Hook" />（<c>IterateHookListeners</c>、
    ///     <c>InvokeExecutionFinished</c>）。
    ///     参数语义对齐 <see cref="MegaCrit.Sts2.Core.Commands.PowerCmd.ModifyAmount" /> 等 Cmd：<paramref name="crafter" /> 为执行合成的单位，
    ///     <paramref name="applier" /> 为引发生效的单位（可为空），<paramref name="cardSource" /> 为引发生效的卡牌（可为空）。
    /// </summary>
    public static class CraftHook
    {
        public static async Task BeforeCraftIntoHand(
            CombatState combatState,
            PlayerChoiceContext choiceContext,
            Creature crafter,
            Creature? applier,
            CardModel? cardSource)
        {
            foreach (var model in combatState.IterateHookListeners())
            {
                if (model is ICraftIntoHandListener listener)
                    await listener.BeforeCraftIntoHand(choiceContext, crafter, applier, cardSource);

                model.InvokeExecutionFinished();
            }
        }

        public static async Task BeforeCraftProductAddToCombat(CombatState combatState, Creature crafter,
            CardModel product)
        {
            foreach (var model in combatState.IterateHookListeners())
            {
                if (model is ICraftIntoHandListener listener)
                    await listener.BeforeCraftProductAddToCombat(crafter, product);

                model.InvokeExecutionFinished();
            }
        }

        public static async Task AfterCraftProductAddToCombat(CombatState combatState, Creature crafter,
            CardModel product)
        {
            foreach (var model in combatState.IterateHookListeners())
            {
                if (model is ICraftIntoHandListener listener)
                    await listener.AfterCraftProductAddToCombat(crafter, product);

                model.InvokeExecutionFinished();
            }
        }
    }
}
