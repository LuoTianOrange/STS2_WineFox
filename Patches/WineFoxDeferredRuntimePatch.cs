using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    public sealed class WineFoxLocManagerInitializedPatch : IPatchMethod
    {
        public static string PatchId => "winefox_deferred_runtime_after_loc_manager_initialized";
        public static bool IsCritical => true;
        public static string Description => "Apply WineFox runtime patches that require initialized localization";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(LocManager), nameof(LocManager.Initialize))];
        }

        public static void Postfix()
        {
            Main.ApplyDeferredRuntimePatches();
        }
    }
}
