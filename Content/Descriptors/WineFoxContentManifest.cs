using STS2_WineFox.Cards;
using STS2_WineFox.Cards.Basic;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Token;
using STS2_WineFox.Cards.Uncommon;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2_WineFox.Relics;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Content.Descriptors
{
    internal static class WineFoxContentManifest
    {
        private const string Root = "res://STS2_WineFox";

        public static IReadOnlyList<IContentRegistrationEntry> ContentEntries { get; } =
        [
            new CharacterRegistrationEntry<WineFox>(),

            new CardRegistrationEntry<WineFoxCardPool, WineFoxStrike>(),
            new CardRegistrationEntry<WineFoxCardPool, WineFoxDefend>(),
            new CardRegistrationEntry<WineFoxCardPool, BasicMine>(),
            new CardRegistrationEntry<WineFoxCardPool, BaseCraft>(),
            new CardRegistrationEntry<WineFoxCardPool, StonePickaxe>(),
            new CardRegistrationEntry<WineFoxCardPool, FullAttack>(),
            new CardRegistrationEntry<WineFoxCardPool, MiningGems>(),
            new CardRegistrationEntry<WineFoxCardPool, PlantTrees>(),
            new CardRegistrationEntry<WineFoxCardPool, SteamEngine>(),
            new CardRegistrationEntry<WineFoxCardPool, StoneSword>(),
            new CardRegistrationEntry<WineFoxCardPool, IronArmor>(),
            new CardRegistrationEntry<WineFoxCardPool, MechanicalDrill>(),
            new CardRegistrationEntry<WineFoxCardPool, WoodenSword>(),
            new CardRegistrationEntry<WineFoxCardPool, IronPickaxe>(),
            new CardRegistrationEntry<WineFoxCardPool, DiamondSword>(),
            new CardRegistrationEntry<WineFoxCardPool, MechanicalSaw>(),
            new CardRegistrationEntry<WineFoxCardPool, IronSword>(),
            new CardRegistrationEntry<WineFoxCardPool, AlterPath>(),
            new CardRegistrationEntry<WineFoxCardPool, StoneArmor>(),
            new CardRegistrationEntry<WineFoxCardPool, PowerUp>(),
            new CardRegistrationEntry<WineFoxCardPool, IronZombie>(),
            new CardRegistrationEntry<WineFoxCardPool, CrushingWheel>(),
            new CardRegistrationEntry<WineFoxCardPool, EnmergencyRepair>(),
            new CardRegistrationEntry<WineFoxCardPool, LightAssault>(),

            new RelicRegistrationEntry<WineFoxRelicPool, HandCrank>(),
            new RelicRegistrationEntry<WineFoxRelicPool, MaidBackpack>(),

            new PowerRegistrationEntry<StressPower>(),
            new PowerRegistrationEntry<DiggingPower>(),
            new PowerRegistrationEntry<WoodPower>(),
            new PowerRegistrationEntry<PlantPower>(),
            new PowerRegistrationEntry<StonePower>(),
            new PowerRegistrationEntry<IronPower>(),
            new PowerRegistrationEntry<SteamPower>(),
            new PowerRegistrationEntry<StoneSwordPower>(),
            new PowerRegistrationEntry<WoodenSwordPower>(),
            new PowerRegistrationEntry<IronPickaxePower>(),
            new PowerRegistrationEntry<DiamondPower>(),
            new PowerRegistrationEntry<DiamondSwordPower>(),
            new PowerRegistrationEntry<IronSwordPower>(),
            new PowerRegistrationEntry<StoneArmorPower>(),
            new PowerRegistrationEntry<RepairPower>(),
        ];

        public static IReadOnlyList<KeywordRegistrationEntry> KeywordEntries { get; } =
        [
            KeywordRegistrationEntry.Card(WineFoxKeywords.Digging, "STS2_WINEFOX-DIGGING",
                $"{Root}/powers/digging.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Wood, "STS2_WINEFOX-WOOD", $"{Root}/powers/wood_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Stone, "STS2_WINEFOX-STONE",
                $"{Root}/powers/stone_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Plant, "STS2_WINEFOX-PLANT",
                $"{Root}/powers/plant_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Steam, "STS2_WINEFOX-STEAM",
                $"{Root}/powers/steam_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Stress, "STS2_WINEFOX-STRESS",
                $"{Root}/powers/stress_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Iron, "STS2_WINEFOX-IRON", $"{Root}/powers/iron_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Diamond, "STS2_WINEFOX-DIAMOND",
                $"{Root}/powers/diamond_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Strength, "STS2_WINEFOX-STRENGTH",
                "res://images/powers/strength_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Plating, "STS2_WINEFOX-PLATING",
                "res://images/powers/plating_power.png"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Material, "STS2_WINEFOX-MATERIAL")
        ];
    }
}
