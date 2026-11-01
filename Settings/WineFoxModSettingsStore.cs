using STS2RitsuLib;
using STS2RitsuLib.Utils.Persistence;

namespace STS2_WineFox.Settings
{
    public static class WineFoxModSettingsStore
    {
        public const string DataKey = "settings";
        private const string FileName = "winefox_settings.json";

        private static bool _initialized;

        public static bool EventsEnabled => Current.EventsEnabled;
        public static bool PotionsEnabled => Current.PotionsEnabled;
        public static bool FoodEnabled => Current.FoodEnabled;
        public static bool PublicRelicsEnabled => Current.PublicRelicsEnabled;

        public static bool FlatCardArtEnabled => Current.FlatCardArtEnabled;

        public static bool IsFlatCardArtEnabled(string cardKey)
        {
            return Current.FlatCardArtOverrides.TryGetValue(cardKey, out var value)
                ? value
                : Current.FlatCardArtEnabled;
        }

        public static bool? GetFlatCardArtOverride(string cardKey)
        {
            return Current.FlatCardArtOverrides.TryGetValue(cardKey, out var value) ? value : null;
        }

        public static void SetFlatCardArtOverride(string cardKey, bool? useFlat)
        {
            var store = RitsuLibFramework.GetDataStore(Const.ModId);

            store.Modify<WineFoxModSettings>(DataKey, settings =>
            {
                if (useFlat == null)
                    settings.FlatCardArtOverrides.Remove(cardKey);
                else
                    settings.FlatCardArtOverrides[cardKey] = useFlat.Value;
            });

            store.Save(DataKey);
        }

        public static void SetFlatCardArtForAll(bool useFlat)
        {
            var store = RitsuLibFramework.GetDataStore(Const.ModId);

            store.Modify<WineFoxModSettings>(DataKey, settings =>
            {
                settings.FlatCardArtEnabled = useFlat;
                settings.FlatCardArtOverrides.Clear();
            });

            store.Save(DataKey);
        }

        private static WineFoxModSettings Current
        {
            get
            {
                Initialize();
                return RitsuLibFramework.GetDataStore(Const.ModId).Get<WineFoxModSettings>(DataKey);
            }
        }

        public static void Initialize()
        {
            if (_initialized)
                return;

            using (RitsuLibFramework.BeginModDataRegistration(Const.ModId, false))
            {
                RitsuLibFramework.GetDataStore(Const.ModId).Register(
                    DataKey,
                    FileName,
                    SaveScope.Global,
                    () => new WineFoxModSettings(),
                    true);
            }

            RitsuLibFramework.GetDataStore(Const.ModId).InitializeGlobal();
            _initialized = true;
        }
    }
}
