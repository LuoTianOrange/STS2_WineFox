using STS2_WineFox.Cards;
using STS2_WineFox.Cards.Ancient;
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
            new SharedCardPoolRegistrationEntry<WineFoxTokenCardPool>(),

            //CharacterCardPool
            new CardRegistrationEntry<WineFoxCardPool, WineFoxStrike>(),
            new CardRegistrationEntry<WineFoxCardPool, WineFoxDefend>(),
            new CardRegistrationEntry<WineFoxCardPool, BasicMine>(),
            new CardRegistrationEntry<WineFoxCardPool, BasicCraft>(),
            new CardRegistrationEntry<WineFoxCardPool, FullAttack>(),
            new CardRegistrationEntry<WineFoxCardPool, MiningGems>(),
            new CardRegistrationEntry<WineFoxCardPool, PlantTrees>(),
            new CardRegistrationEntry<WineFoxCardPool, SteamEngine>(),
            new CardRegistrationEntry<WineFoxCardPool, MechanicalDrill>(),
            new CardRegistrationEntry<WineFoxCardPool, MechanicalSaw>(),
            new CardRegistrationEntry<WineFoxCardPool, AlterPath>(),
            new CardRegistrationEntry<WineFoxCardPool, PowerUp>(),
            new CardRegistrationEntry<WineFoxCardPool, IronZombie>(),
            new CardRegistrationEntry<WineFoxCardPool, CrushingWheel>(),
            new CardRegistrationEntry<WineFoxCardPool, EnmergencyRepair>(),
            new CardRegistrationEntry<WineFoxCardPool, LightAssault>(),
            new CardRegistrationEntry<WineFoxCardPool, EasyPeasy>(),
            new CardRegistrationEntry<WineFoxCardPool, AllItem>(),
            new CardRegistrationEntry<WineFoxCardPool, LessHoliday>(),
            new CardRegistrationEntry<WineFoxCardPool, RiclearPowerPlant>(),
            new CardRegistrationEntry<WineFoxCardPool, Alternator>(),
            new CardRegistrationEntry<WineFoxCardPool, NetheritePickaxe>(),
            new CardRegistrationEntry<WineFoxCardPool, CobblestoneGenerator>(),
            new CardRegistrationEntry<WineFoxCardPool, ShieldAttack>(),
            new CardRegistrationEntry<WineFoxCardPool, SpinningHand>(),
            new CardRegistrationEntry<WineFoxCardPool, ProductionDocument>(),
            new CardRegistrationEntry<WineFoxCardPool, RecordPlayer>(),
            new CardRegistrationEntry<WineFoxCardPool, Milk>(),
            new CardRegistrationEntry<WineFoxCardPool, VacantDomain>(),
            new CardRegistrationEntry<WineFoxCardPool, PressWToThink>(),
            new CardRegistrationEntry<WineFoxCardPool, BackupCrafting>(),
            new CardRegistrationEntry<WineFoxCardPool, TicTacToeGrid>(),
            new CardRegistrationEntry<WineFoxCardPool, Traditionalist>(),
            new CardRegistrationEntry<WineFoxCardPool, SlashBladeWood>(),
            new CardRegistrationEntry<WineFoxCardPool, DripstoneTrap>(),
            new CardRegistrationEntry<WineFoxCardPool, PreProcessing>(),
            new CardRegistrationEntry<WineFoxCardPool, WorkbenchBackpack>(),
            new CardRegistrationEntry<WineFoxCardPool, Regroup>(),
            new CardRegistrationEntry<WineFoxCardPool, ChangeEquipment>(),
            new CardRegistrationEntry<WineFoxCardPool, FoxBite>(),
            new CardRegistrationEntry<WineFoxCardPool, MaidSupport>(),
            
            //TokenCardPool
            new CardRegistrationEntry<WineFoxTokenCardPool, StonePickaxe>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, StoneSword>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, IronArmor>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, WoodenSword>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, IronPickaxe>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, DiamondSword>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, IronSword>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, StoneArmor>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, Nothing>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, WoodenPickaxe>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, WoodenArmor>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, WorkWork>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, DiamondArmor>(),

            //RelicPool
            new RelicRegistrationEntry<WineFoxRelicPool, HandCrank>(),
            new RelicRegistrationEntry<WineFoxRelicPool, MaidBackpack>(),
            new RelicRegistrationEntry<WineFoxRelicPool, Deployer>(),
            
            //UpdateRelic
            new TouchOfOrobasRefinementRegistrationEntry<HandCrank,Deployer>(),

            //Power
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
            new PowerRegistrationEntry<RadiationLeakPower>(),
            new PowerRegistrationEntry<EasyPeasyPower>(),
            new PowerRegistrationEntry<NetheritePickaxePower>(),
            new PowerRegistrationEntry<BrushStoneFormPower>(),
            new PowerRegistrationEntry<SlashBladeWoodPower>(),
            new PowerRegistrationEntry<ProductionDocumentPower>(),
            new PowerRegistrationEntry<DiamondArmorPower>(),
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
            KeywordRegistrationEntry.Card(WineFoxKeywords.Material, "STS2_WINEFOX-MATERIAL"),
            KeywordRegistrationEntry.Card(WineFoxKeywords.Craft, "STS2_WINEFOX-CRAFT"),
        ];
    }
}
