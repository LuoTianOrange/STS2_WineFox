using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Saves.Managers;

namespace STS2_WineFox.Patches
{
    [HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenElitesDefeatedEpoch")]
    internal static class ProgressSaveManagerPatch
    {
        static bool Prefix(Player localPlayer) => false;
    }
}