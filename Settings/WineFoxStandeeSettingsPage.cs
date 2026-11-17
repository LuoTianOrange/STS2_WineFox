using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Settings
{
    public static class WineFoxStandeeSettingsPage
    {
        private const string PageId = "standee";
        private const string EntryId = "standee_variant";

        private static readonly Lazy<I18N> Localization = new(() => RitsuLibFramework.CreateModLocalization(
            Const.ModId,
            "WineFox-ModSettings",
            pckFolders: [$"res://{Const.ModId}/localization/mod_settings"]));

        public static void Register()
        {
            RitsuLibFramework.RegisterModSettings(Const.ModId, page => page
                .WithTitle(T("winefox.standee.subpage.title", "WineFox Standee"))
                .WithDescription(T("winefox.standee.subpage.description",
                    "Switch the WineFox battle standee between the new and the legacy skeleton. MagicWineFox is not affected."))
                .WithModDisplayName(T("winefox.settings.mod.displayName", "WineFox"))
                .WithSortOrder(11)
                .AddSection("variant", section => section
                    .WithTitle(T("winefox.standee.section.title", "Battle standee"))
                    .AddChoice(
                        EntryId,
                        T("winefox.standee.variant.label", "Standee"),
                        Binding(),
                        [
                            new(WineFoxStandeeVariants.New, T("winefox.standee.variant.new", "New standee")),
                            new(WineFoxStandeeVariants.Old, T("winefox.standee.variant.old", "Legacy standee")),
                        ],
                        T("winefox.standee.variant.description",
                            "Takes effect from the next combat. The legacy standee has no low-health or death animation, so it keeps idling at low health and shows a falling pose on death."))),
                PageId);
        }

        private static IModSettingsValueBinding<string> Binding()
        {
            return ModSettingsBindings.Callback(
                Const.ModId,
                EntryId,
                () => WineFoxModSettingsStore.StandeeVariant,
                value =>
                {
                    if (RunManager.Instance?.IsInProgress == true)
                        return;

                    WineFoxModSettingsStore.SetStandeeVariant(value);
                    WineFoxStandeeSync.PublishLocal();
                },
                static () => { });
        }

        private static ModSettingsText T(string key, string fallback)
        {
            return ModSettingsText.I18N(Localization.Value, key, fallback);
        }
    }
}
