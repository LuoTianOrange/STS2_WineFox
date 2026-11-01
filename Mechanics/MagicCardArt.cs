using Godot;
using STS2_WineFox.Settings;

namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     魔法酒狐卡面在正式版（magic_common）与扁平版（magic_flat）之间的运行时解析。
    ///     其它卡面路径原样返回，不受影响。
    /// </summary>
    public static class MagicCardArt
    {
        public const string CommonSegment = "/magic_winefox/magic_common/";

        public const string FlatSegment = "/magic_winefox/magic_flat/";

        public static string Resolve(string portraitPath)
        {
            if (string.IsNullOrEmpty(portraitPath) ||
                !portraitPath.Contains(CommonSegment, StringComparison.Ordinal))
                return portraitPath;

            if (!WineFoxModSettingsStore.IsFlatCardArtEnabled(KeyOf(portraitPath)))
                return portraitPath;

            var flat = portraitPath.Replace(CommonSegment, FlatSegment, StringComparison.Ordinal);

            return ResourceLoader.Exists(flat) ? flat : portraitPath;
        }

        public static bool HasFlatVariant(string portraitPath)
        {
            if (string.IsNullOrEmpty(portraitPath) ||
                !portraitPath.Contains(CommonSegment, StringComparison.Ordinal))
                return false;

            return ResourceLoader.Exists(
                portraitPath.Replace(CommonSegment, FlatSegment, StringComparison.Ordinal));
        }

        public static string CommonPathOf(string portraitPath)
        {
            return portraitPath.Replace(FlatSegment, CommonSegment, StringComparison.Ordinal);
        }

        public static string KeyOf(string portraitPath)
        {
            return Path.GetFileNameWithoutExtension(portraitPath);
        }
    }
}
