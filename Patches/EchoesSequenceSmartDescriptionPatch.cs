using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Powers;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
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
            switch (__instance)
            {
                case EchoesSequencePower echo:
                    echo.AddBoundSpellArg(__result);
                    break;
                case ReproductionPower reproduction:
                    reproduction.AddBoundSpellArg(__result);
                    break;
            }
        }
    }
}
