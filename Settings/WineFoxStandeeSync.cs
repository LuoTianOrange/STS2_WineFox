using System.Text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using STS2_WineFox.Character;
using STS2_WineFox.Network;

namespace STS2_WineFox.Settings
{
    internal static class WineFoxStandeeSync
    {
        private static readonly Lock Gate = new();
        private static Dictionary<string, string> _variants = [];

        public static void Initialize()
        {
            _variants = [];
        }

        public static string VariantFor(Player? player)
        {
            var fallback = WineFoxModSettingsStore.StandeeVariant;
            if (player == null)
                return fallback;

            lock (Gate)
            {
                return _variants.TryGetValue(Key(player.NetId), out var variant)
                    ? WineFoxStandeeVariants.Normalize(variant)
                    : fallback;
            }
        }

        public static bool UsesOldStandee(Player? player)
        {
            return WineFoxStandeeVariants.IsOld(VariantFor(player));
        }

        public static void PublishLocal()
        {
        }

        public static void TryEnsurePublished()
        {
        }

        public static void ResetRemote()
        {
            lock (Gate)
            {
                if (TryGetLocalNetId(out var netId))
                    _variants.Remove(Key(netId));
                else
                    _variants.Clear();
            }
        }

        public static byte[]? EncodeLocal()
        {
            var netId = WineFoxStandeeWire.CurrentSenderId;
            if (netId == 0UL && !TryGetLocalNetId(out netId))
                return null;

            return Encoding.UTF8.GetBytes(Key(netId) + "=" + WineFoxModSettingsStore.StandeeVariant);
        }

        public static byte[]? EncodeAll()
        {
            if (!TryGetLocalNetId(out var selfId))
                selfId = WineFoxStandeeWire.CurrentSenderId;
            if (selfId == 0UL)
                return EncodeLocal();

            Dictionary<string, string> snapshot;
            lock (Gate)
            {
                snapshot = new Dictionary<string, string>(_variants)
                {
                    [Key(selfId)] = WineFoxModSettingsStore.StandeeVariant
                };
            }

            var text = string.Join(';', snapshot.Select(static pair => pair.Key + "=" + pair.Value));
            return Encoding.UTF8.GetBytes(text);
        }

        public static void ApplyPayload(int version, ReadOnlyMemory<byte> payload)
        {
            if (payload.IsEmpty)
                return;

            var text = Encoding.UTF8.GetString(payload.Span);
            var applied = new Dictionary<string, string>();

            foreach (var entry in text.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var split = entry.IndexOf('=');
                if (split <= 0 || split >= entry.Length - 1)
                    continue;

                applied[entry[..split]] = WineFoxStandeeVariants.Normalize(entry[(split + 1)..]);
            }

            if (applied.Count == 0)
                return;

            lock (Gate)
            {
                foreach (var (key, value) in applied)
                    _variants[key] = value;
            }

            WineFoxStandee.RefreshExisting();
        }

        private static bool TryGetLocalNetId(out ulong netId)
        {
            var netService = RunManager.Instance?.NetService;
            if (netService == null)
            {
                netId = 0UL;
                return false;
            }

            try
            {
                netId = netService.NetId;
                return true;
            }
            catch (InvalidOperationException)
            {
                netId = 0UL;
                return false;
            }
        }

        private static string Key(ulong netId)
        {
            return netId.ToString();
        }
    }
}
