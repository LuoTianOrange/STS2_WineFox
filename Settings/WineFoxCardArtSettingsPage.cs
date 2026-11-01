using System.Reflection;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils;
using STS2RitsuLib.Utils.Persistence;
using STS2_WineFox.Mechanics;

namespace STS2_WineFox.Settings
{
    public static class WineFoxCardArtSettingsPage
    {
        private const string PageId = "card_art";

        private static readonly Lazy<I18N> Localization = new(() => RitsuLibFramework.CreateModLocalization(
            Const.ModId,
            "WineFox-ModSettings",
            pckFolders: [$"res://{Const.ModId}/localization/mod_settings"]));

        private static readonly Lazy<IReadOnlyList<(string Key, string Path, string Field)>> MagicPortraits =
            new(CollectMagicPortraits);

        public static void Register()
        {
            RitsuLibFramework.RegisterModSettings(Const.ModId, page => page
                .WithTitle(T("winefox.cardArt.subpage.title", "MagicWineFox Card Art"))
                .WithDescription(T("winefox.cardArt.subpage.description",
                    "Switch MagicWineFox card portraits between the full-art version and the flat-color version."))
                .WithModDisplayName(T("winefox.settings.mod.displayName", "WineFox"))
                .WithSortOrder(10)
                .AddSection("style", section => BuildStyleSection(section))
                .AddSection("cards", section => BuildCardSection(section)),
                PageId);
        }

        private static void BuildStyleSection(ModSettingsSectionBuilder section)
        {
            section
                .WithTitle(T("winefox.cardArt.section.style.title", "Overall style"))
                .AddToggle(
                    "flat_card_art_default",
                    T("winefox.cardArt.default.label", "Use flat card art by default"),
                    DefaultBinding(),
                    T("winefox.cardArt.default.description",
                        "New cards use the flat-color portrait unless a per-card override says otherwise."))
                .AddButton(
                    "flat_card_art_all",
                    T("winefox.cardArt.all.label", "Switch every card to flat"),
                    T("winefox.cardArt.all.button", "Switch all"),
                    host => ApplyToAll(host, true),
                    ModSettingsButtonTone.Normal,
                    T("winefox.cardArt.all.description",
                        "Clear per-card overrides and use flat art for every MagicWineFox card."))
                .AddButton(
                    "flat_card_art_restore",
                    T("winefox.cardArt.restore.label", "Restore full art"),
                    T("winefox.cardArt.restore.button", "Restore"),
                    host => ApplyToAll(host, false),
                    ModSettingsButtonTone.Danger,
                    T("winefox.cardArt.restore.description",
                        "Clear per-card overrides and return every card to the full-art version."));
        }

        private static void BuildCardSection(ModSettingsSectionBuilder section)
        {
            section.WithTitle(T("winefox.cardArt.section.cards.title", "Per-card override"));

            foreach (var (key, _, field) in MagicPortraits.Value)
                section.AddToggle(
                    "flat_" + key,
                    ModSettingsText.Literal(DisplayNameOf(field, key)),
                    CardBinding(key),
                    T("winefox.cardArt.card.description", "Use the flat-color portrait for this card."));
        }

        private static void ApplyToAll(IModSettingsUiActionHost host, bool useFlat)
        {
            if (!CanEditSettings())
                return;

            WineFoxModSettingsStore.SetFlatCardArtForAll(useFlat);

            foreach (var (key, _, _) in MagicPortraits.Value)
                host.MarkDirty(CardBinding(key));

            host.MarkDirty(DefaultBinding());
            host.RequestRefreshAfterDataModelBatchChange();
        }

        private static IModSettingsValueBinding<bool> DefaultBinding()
        {
            return ModSettingsBindings.Callback(
                Const.ModId,
                "flat_card_art_default",
                () => WineFoxModSettingsStore.FlatCardArtEnabled,
                value =>
                {
                    if (CanEditSettings())
                        WineFoxModSettingsStore.SetFlatCardArtForAll(value);
                },
                static () => { });
        }

        private static IModSettingsValueBinding<bool> CardBinding(string cardKey)
        {
            return ModSettingsBindings.Callback(
                Const.ModId,
                "flat_card_" + cardKey,
                () => WineFoxModSettingsStore.IsFlatCardArtEnabled(cardKey),
                value =>
                {
                    if (CanEditSettings())
                        WineFoxModSettingsStore.SetFlatCardArtOverride(cardKey, value);
                },
                static () => { });
        }

        private static bool CanEditSettings()
        {
            var runManager = RunManager.Instance;

            return runManager?.IsInProgress != true;
        }

        private static IReadOnlyList<(string Key, string Path, string Field)> CollectMagicPortraits()
        {
            var result = new List<(string, string, string)>();

            foreach (var field in typeof(Const.Paths).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType != typeof(string))
                    continue;

                if (field.GetValue(null) is not string path)
                    continue;

                if (!path.Contains(MagicCardArt.CommonSegment, StringComparison.Ordinal))
                    continue;

                if (!MagicCardArt.HasFlatVariant(path))
                    continue;

                result.Add((MagicCardArt.KeyOf(path), path, field.Name));
            }

            result.Sort((a, b) => string.CompareOrdinal(a.Item1, b.Item1));

            return result;
        }

        private static string DisplayNameOf(string fieldName, string fallbackKey)
        {
            try
            {
                var snake = ToUpperSnake(fieldName);

                var title = new LocString("cards", $"STS2_WINE_FOX_CARD_{snake}.title").GetFormattedText();

                if (!string.IsNullOrWhiteSpace(title) && !title.StartsWith("STS2_WINE_FOX_", StringComparison.Ordinal))
                    return title;
            }
            catch
            {
                // 取不到本地化标题时退回卡面文件名。
            }

            return fallbackKey;
        }

        private static string ToUpperSnake(string fieldName)
        {
            var name = fieldName.StartsWith("Card", StringComparison.Ordinal) ? fieldName[4..] : fieldName;
            var sb = new System.Text.StringBuilder(name.Length + 8);

            for (var i = 0; i < name.Length; i++)
            {
                var c = name[i];

                if (i > 0 && char.IsUpper(c) && name[i - 1] is not '_' &&
                    (char.IsLower(name[i - 1]) || (i + 1 < name.Length && char.IsLower(name[i + 1]))))
                    sb.Append('_');

                sb.Append(char.ToUpperInvariant(c));
            }

            return sb.ToString();
        }

        private static ModSettingsText T(string key, string fallback)
        {
            return ModSettingsText.I18N(Localization.Value, key, fallback);
        }
    }
}
