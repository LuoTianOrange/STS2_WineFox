using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Patching.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.WineFoxCard;

namespace STS2_WineFox.Patches
{
    public sealed class WineFoxStandeeDeathPortraitPatch : IPatchMethod
    {
        public static string PatchId => "winefox_standee_death_portrait";
        public static bool IsCritical => false;
        public static string Description => "Show the die portrait for the legacy WineFox standee";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCreature), nameof(NCreature.StartDeathAnim))];
        }

        public static void Postfix(NCreature __instance, ref float __result)
        {
            if (__instance?.Entity == null)
                return;

            WineFoxStandee.ShowDiePortrait(__instance);

            if (WineFoxStandee.IsWineFoxCharacter(__instance.Entity))
                __result = Mathf.Max(__result, 1.2f);
        }
    }

    public sealed class WineFoxStandeeRevivePortraitPatch : IPatchMethod
    {
        public static string PatchId => "winefox_standee_revive_portrait";
        public static bool IsCritical => false;
        public static string Description => "Restore the spine standee when the legacy WineFox standee revives";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCreature), nameof(NCreature.StartReviveAnim))];
        }

        public static void Postfix(NCreature __instance)
        {
            if (__instance?.Entity == null)
                return;

            WineFoxStandee.HideDiePortrait(__instance);
        }
    }
}
