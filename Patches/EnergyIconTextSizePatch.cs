using System.Reflection;
using HarmonyLib;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    internal sealed class EnergyIconTextSizePatch : IPatchMethod
    {
        private const string TargetMethodName = "OverrideTextIconTag";

        public static string PatchId => "winefox_energy_icon_text_size";

        public static string Description =>
            "Give mod energy icons an explicit rich-text display size so high resolution art is not upscaled";

        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets()
        {
            var declaring = FindDeclaringType(TargetMethodName);
            return declaring == null
                ? []
                : [new ModPatchTarget(declaring, TargetMethodName, null, true)];
        }

        public static void Postfix(ref string __result)
        {
            if (string.IsNullOrEmpty(__result) || !__result.Contains("[img]"))
                return;

            __result = __result.Replace("[img]", "[img=" + Const.EnergyIconTextSize + "]");
        }

        private static Type? FindDeclaringType(string methodName)
        {
            foreach (var type in AccessTools.AllTypes())
            {
                const BindingFlags flags =
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

                if (type.GetMethod(methodName, flags, null, [typeof(string), typeof(string)], null) != null)
                    return type;
            }

            return null;
        }
    }
}
