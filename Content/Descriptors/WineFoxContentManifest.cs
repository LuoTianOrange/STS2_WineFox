using STS2_WineFox.Cards;
using STS2_WineFox.Cards.Basic;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Token;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2_WineFox.Relics;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Content.Descriptors
{
    internal static class WineFoxContentManifest
    {
        public static IReadOnlyList<IContentRegistrationEntry> ContentEntries { get; } =
        [
            new CharacterRegistrationEntry<WineFox>(),

            new CardRegistrationEntry<WineFoxCardPool, WineFoxStrike>(),
            new CardRegistrationEntry<WineFoxCardPool, WineFoxDefend>(),
            new CardRegistrationEntry<WineFoxCardPool, BasicMine>(),
            new CardRegistrationEntry<WineFoxCardPool, BaseCraft>(),
            new CardRegistrationEntry<WineFoxCardPool,StonePick>(),
            new CardRegistrationEntry<WineFoxCardPool,FullAttack>(),

            new RelicRegistrationEntry<WineFoxRelicPool, HandCrank>(),

            new PowerRegistrationEntry<StressPower>(),
            new PowerRegistrationEntry<DiggingPower>(),
            new PowerRegistrationEntry<WoodPower>(),
            new PowerRegistrationEntry<StonePower>(),
            new PowerRegistrationEntry<IronPower>(),
        ];

        public static IReadOnlyList<KeywordRegistrationEntry> KeywordEntries { get; } =
        [
            KeywordRegistrationEntry.Card(WineFoxKeywords.Digging, "STS2_WINEFOX-DIGGING"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Wood, "STS2_WINEFOX-WOOD"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Stone, "STS2_WINEFOX-STONE"),
        ];
    }
}
