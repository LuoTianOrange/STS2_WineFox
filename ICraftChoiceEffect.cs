using STS2_WineFox.Commands;

namespace STS2_WineFox
{
    public interface ICraftChoiceEffect
    {
        Task OnCraftChosen(CraftExecutionContext context);
    }
}
