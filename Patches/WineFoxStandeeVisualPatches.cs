using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Patching.Models;
using STS2_WineFox.Settings;

namespace STS2_WineFox.Patches
{
    public sealed class WineFoxStandeeRunStartPublishPatch : IPatchMethod
    {
        public static string PatchId => "winefox_standee_run_start_publish";
        public static bool IsCritical => false;
        public static string Description => "Publish the local WineFox standee variant when a run is set up";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(RunManager), nameof(RunManager.SetUpNewMultiplayer)),
                new(typeof(RunManager), nameof(RunManager.SetUpSavedMultiplayer))
            ];
        }

        public static void Postfix()
        {
            WineFoxStandeeSync.PublishLocal();
        }
    }
}
