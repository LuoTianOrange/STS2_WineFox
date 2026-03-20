using STS2_WineFox.Cards;
using STS2_WineFox.Cards.Basic;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Token;
using STS2_WineFox.Cards.UnCommon;
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
            new CardRegistrationEntry<WineFoxCardPool, StonePick>(),
            new CardRegistrationEntry<WineFoxCardPool, FullAttack>(),
            new CardRegistrationEntry<WineFoxCardPool, MiningGems>(),
            new CardRegistrationEntry<WineFoxCardPool, PlantTrees>(),
            new CardRegistrationEntry<WineFoxCardPool, SteamEngine>(),
            new CardRegistrationEntry<WineFoxCardPool, StoneSword>(),

            new RelicRegistrationEntry<WineFoxRelicPool, HandCrank>(),

            new PowerRegistrationEntry<StressPower>(),
            new PowerRegistrationEntry<DiggingPower>(),
            new PowerRegistrationEntry<WoodPower>(),
            new PowerRegistrationEntry<PlantPower>(),
            new PowerRegistrationEntry<StonePower>(),
            new PowerRegistrationEntry<IronPower>(),
            new PowerRegistrationEntry<SteamPower>(),
            new PowerRegistrationEntry<StoneSwordPower>()
        ];
        private const string Root = "res://STS2_WineFox";
        public static IReadOnlyList<KeywordRegistrationEntry> KeywordEntries { get; } =
        [
            KeywordRegistrationEntry.Card(WineFoxKeywords.Digging, "STS2_WINEFOX-DIGGING",$"{Root}/powers/digging.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Wood, "STS2_WINEFOX-WOOD",$"{Root}/powers/sts2_wine_fox_power_wood_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Stone, "STS2_WINEFOX-STONE",$"{Root}/powers/sts2_wine_fox_power_stone_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Plant, "STS2_WINEFOX-PLANT",$"{Root}/powers/plant_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Steam, "STS2_WINEFOX-STEAM",$"{Root}/powers/steam_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Stress, "STS2_WINEFOX-STRESS",$"{Root}/powers/stress_power.png"),
        ];
    }
}
