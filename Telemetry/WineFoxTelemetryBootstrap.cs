using System.Text.Json.Nodes;
using STS2_WineFox.Character;
using STS2RitsuLib;
using STS2RitsuLib.Content;
using STS2RitsuLib.Telemetry;

namespace STS2_WineFox.Telemetry
{
    internal static class WineFoxTelemetryBootstrap
    {
        private const string ApplicantId = Const.ModId;
        private const string BackendEndpoint = "https://winefox-telemetry.ritsukage.com/v1/ingest";
        private const string RunContextContributionId = "winefox_run_context";
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            TelemetryRegistry.RegisterContributionProvider(new WineFoxRunContextContribution());
            RitsuLibFramework.RegisterTelemetryApplicant(new()
            {
                ApplicantId = ApplicantId,
                OwnerModId = Const.ModId,
                DisplayName = Const.Name,
                Adapter = new HttpJsonTelemetryAdapter(BackendEndpoint),
                Requests =
                [
                    TelemetryRequest.RunHistory(
                        "WineFox run history and WineFox version for balance and compatibility analysis.",
                        [RunContextContributionId],
                        IsWineFoxRun),
                ],
            });
        }

        private static bool IsWineFoxRun(RunEndedEvent evt)
        {
            var wineFoxEntry = ModContentRegistry.GetFixedPublicEntry(Const.ModId, typeof(WineFox));
            return evt.Run.Players.Any(player =>
                string.Equals(player.CharacterId?.Entry, wineFoxEntry, StringComparison.OrdinalIgnoreCase));
        }

        private sealed class WineFoxRunContextContribution : ITelemetryContributionProvider
        {
            public string ContributorModId => Const.ModId;
            public string ContributionId => RunContextContributionId;
            public TelemetryDataCategory Category => TelemetryDataCategory.RunHistory;
            public TelemetryContributionVisibility Visibility => TelemetryContributionVisibility.PrivateToApplicant;

            public JsonNode Build(TelemetryContributionContext context)
            {
                return new JsonObject
                {
                    ["winefox_version"] = Const.Version,
                };
            }
        }
    }
}
