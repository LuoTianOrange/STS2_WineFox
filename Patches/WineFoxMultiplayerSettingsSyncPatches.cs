using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using STS2_WineFox.Settings;
using STS2RitsuLib.Patching.Core;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    internal static class WineFoxMultiplayerSettingsSyncPatches
    {
        public static void AddTo(ModPatcher patcher)
        {
            patcher.RegisterPatch<StartENetHostSettingsSyncPatch>();
            patcher.RegisterPatch<StartSteamHostSettingsSyncPatch>();
            patcher.RegisterPatch<HostPeerReadySettingsSyncPatch>();
            patcher.RegisterPatch<ClientInitializeSettingsResetPatch>();
            patcher.RegisterPatch<ClientDisconnectedSettingsResetPatch>();
        }

        private sealed class StartENetHostSettingsSyncPatch : IPatchMethod
        {
            public static string PatchId => "winefox_start_enet_host_settings_sync";
            public static bool IsCritical => true;
            public static string Description => "Broadcast WineFox settings when ENet host starts";

            public static ModPatchTarget[] GetTargets()
            {
                return [new(typeof(NetHostGameService), nameof(NetHostGameService.StartENetHost))];
            }

            private static void Prefix(NetHostGameService __instance)
            {
                WineFoxRuntimeSettings.PublishHostSettings(__instance, "start_enet_host");
            }
        }

        private sealed class StartSteamHostSettingsSyncPatch : IPatchMethod
        {
            public static string PatchId => "winefox_start_steam_host_settings_sync";
            public static bool IsCritical => true;
            public static string Description => "Broadcast WineFox settings when Steam host starts";

            public static ModPatchTarget[] GetTargets()
            {
                return [new(typeof(NetHostGameService), nameof(NetHostGameService.StartSteamHost))];
            }

            private static void Prefix(NetHostGameService __instance)
            {
                WineFoxRuntimeSettings.PublishHostSettings(__instance, "start_steam_host");
            }
        }

        private sealed class HostPeerReadySettingsSyncPatch : IPatchMethod
        {
            public static string PatchId => "winefox_host_peer_ready_settings_sync";
            public static bool IsCritical => true;
            public static string Description => "Broadcast WineFox settings after a peer becomes broadcast-ready";

            public static ModPatchTarget[] GetTargets()
            {
                return [new(typeof(NetHostGameService), nameof(NetHostGameService.SetPeerReadyForBroadcasting))];
            }

            private static void Postfix(NetHostGameService __instance)
            {
                WineFoxRuntimeSettings.PublishHostSettings(__instance, "peer_ready");
            }
        }

        private sealed class ClientInitializeSettingsResetPatch : IPatchMethod
        {
            public static string PatchId => "winefox_client_initialize_settings_reset";
            public static bool IsCritical => true;
            public static string Description => "Clear cached WineFox host settings before a client connection starts";

            public static ModPatchTarget[] GetTargets()
            {
                return [new(typeof(NetClientGameService), nameof(NetClientGameService.Initialize))];
            }

            private static void Prefix()
            {
                WineFoxRuntimeSettings.ClearRemoteHostSettings();
            }
        }

        private sealed class ClientDisconnectedSettingsResetPatch : IPatchMethod
        {
            public static string PatchId => "winefox_client_disconnected_settings_reset";
            public static bool IsCritical => true;
            public static string Description => "Clear cached WineFox host settings after client disconnect";

            public static ModPatchTarget[] GetTargets()
            {
                return [new(typeof(NetClientGameService), nameof(NetClientGameService.OnDisconnectedFromHost))];
            }

            private static void Postfix()
            {
                WineFoxRuntimeSettings.ClearRemoteHostSettings();
            }
        }
    }
}
