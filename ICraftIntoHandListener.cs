using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Commands;

namespace STS2_WineFox
{
    /// <summary>
    ///     通用合成生命周期监听器。一次合成会先进入 <see cref="BeforeCraft"/>，成功扣除材料并确定产物后，再按交付模式进入
    ///     <see cref="BeforeCraftProductDelivered"/> / <see cref="AfterCraftProductDelivered"/>。
    /// </summary>
    public interface ICraftListener
    {
        Task BeforeCraft(CraftExecutionContext context)
        {
            return Task.CompletedTask;
        }

        Task BeforeCraftProductDelivered(CraftExecutionContext context)
        {
            return Task.CompletedTask;
        }

        Task AfterCraftProductDelivered(CraftExecutionContext context)
        {
            return Task.CompletedTask;
        }
    }

    /// <summary>
    ///     兼容旧的“合成进手牌”监听接口；内部桥接到 <see cref="ICraftListener"/>。
    /// </summary>
    public interface ICraftIntoHandListener : ICraftListener
    {
        Task BeforeCraftIntoHand(
            PlayerChoiceContext choiceContext,
            Creature crafter,
            Creature? applier,
            CardModel? cardSource)
        {
            return Task.CompletedTask;
        }

        Task BeforeCraftProductAddToCombat(Creature crafter, CardModel product)
        {
            return Task.CompletedTask;
        }

        Task AfterCraftProductAddToCombat(Creature crafter, CardModel product)
        {
            return Task.CompletedTask;
        }

        Task ICraftListener.BeforeCraft(CraftExecutionContext context)
        {
            return BeforeCraftIntoHand(context.ChoiceContext, context.Crafter, context.Applier, context.SourceCard);
        }

        Task ICraftListener.BeforeCraftProductDelivered(CraftExecutionContext context)
        {
            if (context.Product == null)
                return Task.CompletedTask;

            return BeforeCraftProductAddToCombat(context.Crafter, context.Product);
        }

        Task ICraftListener.AfterCraftProductDelivered(CraftExecutionContext context)
        {
            if (context.Product == null)
                return Task.CompletedTask;

            return AfterCraftProductAddToCombat(context.Crafter, context.Product);
        }
    }
}
