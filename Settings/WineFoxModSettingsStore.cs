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
                    defaultFactory: () => new WineFoxModSettings(),
                    autoCreateIfMissing: true);
            }

            RitsuLibFramework.GetDataStore(Const.ModId).InitializeGlobal();
            _initialized = true;
        }

        private static WineFoxModSettings Current
        {
            get
            {
                Initialize();
                return RitsuLibFramework.GetDataStore(Const.ModId).Get<WineFoxModSettings>(DataKey);
            }
        }
    }
}
