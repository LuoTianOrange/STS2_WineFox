using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    /// <summary>
    ///     让【序列回响】的 <c>smartDescription</c> 显示当前绑定的法术名称。
    ///     <para>
    ///         <c>PowerModel.SmartDescription</c> 不是 virtual（无法覆写），
    ///         因此像 RitsuLib 一样用 Harmony 后置补丁，把 <c>{BoundSpell}</c> 填上。
    ///     </para>
    /// </summary>
    internal sealed class EchoesSequenceSmartDescriptionPatch : IPatchMethod
    {
        public static string PatchId => "winefox_echoes_sequence_smart_description";
        public static string Description => "Inject the bound spell's name into EchoesSequencePower's smart description";
        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(PowerModel), nameof(PowerModel.SmartDescription), MethodType.Getter)];
        }

        public static void Postfix(PowerModel __instance, ref LocString __result)
        {
            if (__instance is EchoesSequencePower echo)
                echo.AddBoundSpellArg(__result);
        }
    }
}
