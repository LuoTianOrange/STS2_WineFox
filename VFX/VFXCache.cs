using Godot;

namespace STS2_WineFox.Utils
{
    /// <summary>
    /// 这是一套自带的缓存系统，因为PreloadManager的缓存机制不好注入，所以暂时自己处理
    /// </summary>
    public static class VFXCache
    {
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<string, PackedScene> ModSceneCache =
            new();

        private static List<string> CollectAssetPathsSafely()
        {
            return [
                Const.Paths.DripstoneVfx,
                Const.Paths.BasicMineVfx,
                Const.Paths.HoldWToPonderVfx,
                Const.Paths.DrinkMilkVfx,
            ];
        }

        public static void LoadScenes() {
            try {
                var paths = CollectAssetPathsSafely();
                if (paths.Count > 0) {
                    Main.Logger.Info($"Preloading {paths.Count} assets synchronously");
                    int success = 0, fail = 0;
                    foreach (var path in paths) {
                        try {
                            if (ModSceneCache.ContainsKey(path)) continue;
                            var scene = ResourceLoader.Load<PackedScene>(path, null, ResourceLoader.CacheMode.Reuse);
                            if (scene != null) {
                                ModSceneCache[path] = scene;
                                success++;
                            } else {
                                fail++;
                                Main.Logger.Warn($"Failed to preload: {path}");
                            }
                        } catch (Exception ex) {
                            fail++;
                            Main.Logger.Warn($"Error preloading {path}: {ex.Message}");
                        }
                    }
                    Main.Logger.Info($"Preloading complete: {success} succeeded, {fail} failed");
                }
            }
            catch (Exception ex) {
                Main.Logger.Warn($"Failed to preload RegentFX assets: {ex.Message}");
            }
        }
    }
}
