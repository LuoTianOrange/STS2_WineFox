using System.Collections.Concurrent;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;
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

        private static readonly ConcurrentDictionary<string, bool> FlatExistsCache = new(StringComparer.Ordinal);

        private static readonly ConcurrentDictionary<string, string> ResolvedCache = new(StringComparer.Ordinal);

        public static string Resolve(string portraitPath)
        {
            if (string.IsNullOrEmpty(portraitPath) ||
                !portraitPath.Contains(CommonSegment, StringComparison.Ordinal))
                return portraitPath;

            if (ResolvedCache.TryGetValue(portraitPath, out var cached))
                return cached;

            var resolved = portraitPath;
            if (WineFoxModSettingsStore.IsFlatCardArtEnabled(KeyOf(portraitPath)) &&
                HasFlatVariant(portraitPath))
                resolved = portraitPath.Replace(CommonSegment, FlatSegment, StringComparison.Ordinal);

            ResolvedCache[portraitPath] = resolved;
            return resolved;
        }

        public static bool HasFlatVariant(string portraitPath)
        {
            if (string.IsNullOrEmpty(portraitPath) ||
                !portraitPath.Contains(CommonSegment, StringComparison.Ordinal))
                return false;

            var flat = portraitPath.Replace(CommonSegment, FlatSegment, StringComparison.Ordinal);

            if (FlatExistsCache.TryGetValue(flat, out var exists))
                return exists;

            exists = ResourceLoader.Exists(flat);
            FlatExistsCache[flat] = exists;
            return exists;
        }

        public static void Invalidate()
        {
            ResolvedCache.Clear();
            FlatExistsCache.Clear();
        }

        public static void RefreshOpenCards()
        {
            if (Engine.GetMainLoop() is not SceneTree tree)
                return;

            RefreshNode(tree.Root);
        }

        private static void RefreshNode(Node node)
        {
            if (node is NCard card && card.Model is { } model)
                card.SetPortraitTexture(model.Portrait);

            foreach (var child in node.GetChildren())
                RefreshNode(child);
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
