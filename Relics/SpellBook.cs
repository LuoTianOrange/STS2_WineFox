using MegaCrit.Sts2.Core.Entities.Relics;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(MagicWineFoxRelicPool))]
    [RegisterCharacterStarterRelic(typeof(MagicWineFox))]
    public class SpellBook : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Starter;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.SpellBookRelicIcon);
    }
}
