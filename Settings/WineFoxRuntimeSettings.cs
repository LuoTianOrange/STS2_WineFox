using System.Text.Json;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib.Networking.Sidecar;

namespace STS2_WineFox.Settings
{
    internal static class WineFoxRuntimeSettings
    {
        private static readonly Lock Gate = new();
        private static bool _initialized;
        private static HostSettingsSnapshot? _remoteHostSettings;

        public static bool EventsEnabled => Current.EventsEnabled;
        public static bool PotionsEnabled => Current.PotionsEnabled;
        public static bool FoodEnabled => Current.FoodEnabled;
        public static bool PublicRelicsEnabled => Current.PublicRelicsEnabled;

        private static HostSettingsSnapshot Current
        {
            get
            {
                var netService = RunManager.Instance?.NetService;
                if (netService is NetClientGameService)
                    lock (Gate)
                    {
                        if (_remoteHostSettings is { } remote)
                            return remote;
                    }

                return BuildLocalSnapshot();
            }
        }

        public static void Initialize()
        {
            lock (Gate)
            {
                if (_initialized)
                    return;

                RegisterTopicFromLocalSettings();
                RitsuLibSidecarConfigSyncService.TopicChanged += OnTopicChanged;
                _initialized = true;
            }
        }

        public static void PublishHostSettings(string reason)
        {
            PublishHostSettings(RunManager.Instance?.NetService, reason);
        }

        public static void PublishHostSettings(INetGameService? netService, string reason)
        {
            if (netService is not NetHostGameService)
                return;

            lock (Gate)
            {
                RegisterTopicFromLocalSettings();
                _remoteHostSettings = null;
            }

            RitsuLibSidecarConfigSyncService.PublishHostState(netService, Const.HostSettingsSyncTopic, 0, reason);
        }

        public static void ClearRemoteHostSettings()
        {
            lock (Gate)
            {
                _remoteHostSettings = null;
            }
        }

        private static void RegisterTopicFromLocalSettings()
        {
            RitsuLibSidecarConfigSyncService.RegisterTopic<HostSettingsSnapshot, HostSettingsSnapshot>(
                Const.HostSettingsSyncTopic,
                BuildLocalSnapshot(),
                (_, _) => false,
                (state, _) => state);
        }

        private static HostSettingsSnapshot BuildLocalSnapshot()
        {
            return new(
                WineFoxModSettingsStore.EventsEnabled,
                WineFoxModSettingsStore.PotionsEnabled,
                WineFoxModSettingsStore.FoodEnabled,
                WineFoxModSettingsStore.PublicRelicsEnabled);
        }

        private static void OnTopicChanged(SidecarConfigTopicChangedEvent ev)
        {
            if (ev.Topic != Const.HostSettingsSyncTopic)
                return;

            if (RunManager.Instance?.NetService is not NetClientGameService)
                return;

            var snapshot = JsonSerializer.Deserialize<HostSettingsSnapshot>(ev.StateJson);
            if (snapshot is null)
                return;

            lock (Gate)
            {
                _remoteHostSettings = snapshot;
            }
        }

        private sealed record HostSettingsSnapshot(
            bool EventsEnabled,
            bool PotionsEnabled,
            bool FoodEnabled,
            bool PublicRelicsEnabled);
    }
}
