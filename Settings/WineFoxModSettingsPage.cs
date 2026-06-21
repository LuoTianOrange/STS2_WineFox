using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils;
using STS2RitsuLib.Utils.Persistence;

namespace STS2_WineFox.Settings
{
    public static class WineFoxModSettingsPage
    {
        private static readonly Lazy<I18N> Localization = new(() => RitsuLibFramework.CreateModLocalization(
            Const.ModId,
            "WineFox-ModSettings",
            pckFolders: ["res://STS2_WineFox/localization/mod_settings"]));

        private static bool _registered;
        private static bool _suppressSettingsPublish;

        public static void Register()
        {
            if (_registered)
                return;

            var eventsBinding = Binding(settings => settings.EventsEnabled, (settings, value) => settings.EventsEnabled = value);
            var potionsBinding = Binding(settings => settings.PotionsEnabled, (settings, value) => settings.PotionsEnabled = value);
            var foodBinding = Binding(settings => settings.FoodEnabled, (settings, value) => settings.FoodEnabled = value);
            var publicRelicsBinding = Binding(settings => settings.PublicRelicsEnabled, (settings, value) => settings.PublicRelicsEnabled = value);

            RitsuLibFramework.RegisterModSettings(Const.ModId, page => page
                .WithTitle(T("winefox.settings.page.title", "WineFox Settings"))
                .WithModDisplayName(T("winefox.settings.mod.displayName", "WineFox"))
                .WithReadOnlyOnHostSurfaces(ModSettingsHostSurface.RunPause | ModSettingsHostSurface.CombatPause)
                .AddSection("content", section => section
                    .WithTitle(T("winefox.settings.section.content.title", "Content"))
                    .AddToggle(
                        "events_enabled",
                        T("winefox.settings.events.label", "Enable WineFox events"),
                        eventsBinding,
                        T("winefox.settings.events.description", "Allow WineFox-specific events to appear in future event rolls."))
                    .AddToggle(
                        "potions_enabled",
                        T("winefox.settings.potions.label", "Enable WineFox potions"),
                        potionsBinding,
                        T("winefox.settings.potions.description", "Allow WineFox normal potions to appear in future potion rolls."))
                    .AddToggle(
                        "food_enabled",
                        T("winefox.settings.food.label", "Enable WineFox food"),
                        foodBinding,
                        T("winefox.settings.food.description", "Allow WineFox food rewards and food-generating effects."))
                    .AddToggle(
                        "public_relics_enabled",
                        T("winefox.settings.publicRelics.label", "Enable public WineFox relics"),
                        publicRelicsBinding,
                        T("winefox.settings.publicRelics.description", "Allow selected WineFox relics to appear in shared relic rewards."))
                    .AddButton(
                        "restore_defaults",
                        T("winefox.settings.restoreDefaults.label", "Restore defaults"),
                        T("winefox.settings.restoreDefaults.button", "Restore"),
                        host => RestoreDefaults(host, eventsBinding, potionsBinding, foodBinding, publicRelicsBinding),
                        ModSettingsButtonTone.Danger,
                        T("winefox.settings.restoreDefaults.description", "Restore all WineFox content settings to their default enabled state."))));

            _registered = true;
        }

        private static void RestoreDefaults(
            IModSettingsUiActionHost host,
            ModSettingsValueBinding<WineFoxModSettings, bool> eventsBinding,
            ModSettingsValueBinding<WineFoxModSettings, bool> potionsBinding,
            ModSettingsValueBinding<WineFoxModSettings, bool> foodBinding,
            ModSettingsValueBinding<WineFoxModSettings, bool> publicRelicsBinding)
        {
            if (!CanEditSettings())
                return;

            var defaults = new WineFoxModSettings();

            _suppressSettingsPublish = true;
            try
            {
                eventsBinding.Write(defaults.EventsEnabled);
                potionsBinding.Write(defaults.PotionsEnabled);
                foodBinding.Write(defaults.FoodEnabled);
                publicRelicsBinding.Write(defaults.PublicRelicsEnabled);
            }
            finally
            {
                _suppressSettingsPublish = false;
            }

            WineFoxRuntimeSettings.PublishHostSettings("restore_defaults");

            host.MarkDirty(eventsBinding);
            host.MarkDirty(potionsBinding);
            host.MarkDirty(foodBinding);
            host.MarkDirty(publicRelicsBinding);
            host.RequestRefreshAfterDataModelBatchChange();
        }

        private static ModSettingsText T(string key, string fallback)
        {
            return ModSettingsText.I18N(Localization.Value, key, fallback);
        }

        private static ModSettingsValueBinding<WineFoxModSettings, bool> Binding(
            Func<WineFoxModSettings, bool> getter,
            Action<WineFoxModSettings, bool> setter)
        {
            return new(
                Const.ModId,
                WineFoxModSettingsStore.DataKey,
                SaveScope.Global,
                getter,
                (settings, value) =>
                {
                    if (!CanEditSettings())
                        return;

                    setter(settings, value);
                    if (!_suppressSettingsPublish)
                        WineFoxRuntimeSettings.PublishHostSettings("settings_changed");
                });
        }

        private static bool CanEditSettings()
        {
            var runManager = RunManager.Instance;
            if (runManager?.IsInProgress == true)
                return false;

            return runManager?.NetService is not NetClientGameService;
        }
    }
}
