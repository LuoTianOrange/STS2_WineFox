using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Powers;

public static class WineFoxActions
{
    /// <summary>
    /// 获得材料的统一入口（木板/圆石/铁锭等）。
    /// 翻倍效果由 HandCrank 的 ModifyPowerAmountGiven 钩子自动处理，无需在此重复检查。
    /// </summary>
    public static async Task GainMaterial<T>(CardModel card, decimal amount)
        where T : PowerModel
    {
        await CommonActions.ApplySelf<T>(card, amount);
    }
}