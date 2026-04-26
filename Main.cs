using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2_WineFox.Commands;
using STS2_WineFox.Content;
using STS2RitsuLib;
using STS2RitsuLib.Audio;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Unlocks;

namespace STS2_WineFox
{
    [ModInitializer(nameof(Initialize))]
    public static class Main
    {
        public static readonly Logger Logger = RitsuLibFramework.CreateLogger(Const.ModId);

        private static IDisposable? _winefoxBankDeferredInitSubscription;
        
        public static bool IsModActive { get; private set; }

        public static void Initialize()
        {
            if (IsModActive)
            {
                Logger.Debug("Mod already initialized, skipping duplicate initialization.");
                return;
            }

            Logger.Info($"Mod ID: {Const.ModId}");
            Logger.Info($"Version: {Const.Version}");
            Logger.Info("Initializing mod...");

            try
            {
                if (Const.IgnoreEpochRequirements)
                    ModUnlockRegistry.SetEpochRequirementsIgnoredForMod(Const.ModId);

                var assembly = Assembly.GetExecutingAssembly();
                RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
                QueueWineFoxFmodBankAfterDeferredInitialization();
                LoadWineFoxFmodBanksAligned();
                ModTypeDiscoveryHub.RegisterModAssembly(Const.ModId, assembly);
                MaterialPowerRegistry.RegisterWineFoxDefaults();
                IsModActive = true;
                Logger.Info("Mod initialization complete - Mod is now ACTIVE");
            }
            catch (Exception ex)
            {
                Logger.Error($"Mod initialization failed with exception: {ex.Message}");
                Logger.Error($"Stack trace: {ex.StackTrace}");
                IsModActive = false;
            }
        }
        
        private static void QueueWineFoxFmodBankAfterDeferredInitialization()
        {
            if (_winefoxBankDeferredInitSubscription != null)
                return;

            _winefoxBankDeferredInitSubscription =
                RitsuLibFramework.SubscribeLifecycle<DeferredInitializationCompletedEvent>(_ =>
                {
                    try
                    {
                        if (FmodStudioServer.TryGet() is null)
                        {
                            Logger.Warn("FmodServer singleton missing; skipped FMOD bank load.");
                            return;
                        }

                        LoadWineFoxFmodBanksAligned();
                    }
                    finally
                    {
                        _winefoxBankDeferredInitSubscription?.Dispose();
                        _winefoxBankDeferredInitSubscription = null;
                    }
                });
        }

        private static void LoadWineFoxFmodBanksAligned()
        {
            if (!FmodStudioServer.TryLoadBank(Const.Paths.WineFoxBank))
            {
                Logger.Warn($"Failed to load FMOD bank: {Const.Paths.WineFoxBank}");
                return;
            }

            FmodStudioServer.TryLoadStudioGuidMappings(Const.Paths.WineFoxGuidsFile);
        }
    }
}
