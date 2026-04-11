using MegaCrit.Sts2.Core.Combat;
using STS2_WineFox.Commands;

namespace STS2_WineFox.Hooks
{
    /// <summary>
    ///     合成流程钩子分发。基于 <see cref="CraftExecutionContext"/> 统一承载一次 craft 的上下文，允许根据不同交付模式扩展。
    /// </summary>
    public static class CraftHook
    {
        public static async Task BeforeCraft(CombatState combatState, CraftExecutionContext context)
        {
            foreach (var model in combatState.IterateHookListeners())
            {
                if (model is ICraftListener listener)
                    await listener.BeforeCraft(context);

                model.InvokeExecutionFinished();
            }
        }

        public static async Task BeforeCraftProductDelivered(CombatState combatState, CraftExecutionContext context)
        {
            foreach (var model in combatState.IterateHookListeners())
            {
                if (model is ICraftListener listener)
                    await listener.BeforeCraftProductDelivered(context);

                model.InvokeExecutionFinished();
            }
        }

        public static async Task AfterCraftProductDelivered(CombatState combatState, CraftExecutionContext context)
        {
            foreach (var model in combatState.IterateHookListeners())
            {
                if (model is ICraftListener listener)
                    await listener.AfterCraftProductDelivered(context);

                model.InvokeExecutionFinished();
            }
        }
    }
}
