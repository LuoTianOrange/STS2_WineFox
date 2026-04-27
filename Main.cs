using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Patches;
using STS2RitsuLib;
using STS2RitsuLib.Audio;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Patching.Core;
using STS2RitsuLib.Unlocks;

namespace STS2_WineFox
{
    [ModInitializer(nameof(Initialize))]
    public static class Main
    {
        public static readonly Logger Logger = RitsuLibFramework.CreateLogger(Const.ModId);

        private static IDisposable? _winefoxBankDeferredInitSubscription;
        private static ModPatcher? _runtimePatcher;

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
                if (!EnsureRequiredRuntimePatches())
                    return;
                QueueWineFoxFmodBankAfterDeferredInitialization();
                LoadWineFoxFmodBanksAligned();
                var wineFoxPublicEntry = ModContentRegistry.GetFixedPublicEntry(Const.ModId, typeof(WineFox));
                RitsuLibFramework.GetContentRegistry(Const.ModId)
                    .RegisterCardLibraryCompendiumSharedPoolFilter<WineFoxCraftingCardPool>(
                        "winefox_crafting",
                        Const.Paths.CraftingCodexTopBarButtonIcon,
                        [
                            new()
                            {
                                ModCharacterModelIdEntry = wineFoxPublicEntry,
                                Relation = CardLibraryCompendiumFilterInsertRelation.After,
                            },
                        ]);
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

        private static bool EnsureRequiredRuntimePatches()
        {
            _runtimePatcher ??= CreateRuntimePatcher();
            return RitsuLibFramework.ApplyRequiredPatcher(
                _runtimePatcher,
                DisableModOnRequiredPatcherFailure,
                "WineFox required runtime patches failed; mod initialization aborted.");
        }

        private static ModPatcher CreateRuntimePatcher()
        {
            var patcher = RitsuLibFramework.CreatePatcher(Const.ModId, "runtime-effects", "runtime effects");
            patcher.RegisterPatch<WineFoxCreatureLoseHpFlashPatch>();
            patcher.RegisterPatch<WineFoxCreatureHitTriggerFlashPatch>();
            patcher.RegisterPatch<WineFoxCreatureDeathSmokePlaceholderPatch>();
            return patcher;
        }

        private static void DisableModOnRequiredPatcherFailure()
        {
            IsModActive = false;
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
