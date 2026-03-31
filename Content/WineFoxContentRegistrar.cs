using STS2_WineFox.Content.Descriptors;
using STS2RitsuLib;

namespace STS2_WineFox.Content
{
    internal static class WineFoxContentRegistrar
    {
        internal static void RegisterAll()
        {
            // Order: ModelDb content → ModKeywordRegistry → timeline/unlocks (RequireEpoch needs character registered).
            RitsuLibFramework.CreateContentPack(Const.ModId)
                .ContentManifest(WineFoxContentManifest.ContentEntries)
                .KeywordManifest(WineFoxContentManifest.KeywordEntries)
                .PackManifest(WineFoxContentManifest.PackEntries)
                //.Custom(WineFoxPlaceholders.Register)
                .Apply();
        }
    }
}
