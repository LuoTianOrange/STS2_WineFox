using MegaCrit.Sts2.Core.Models.Characters;
using STS2_WineFox.Cards;
using STS2_WineFox.Cards.Ancient;
using STS2_WineFox.Cards.Basic;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Token;
using STS2_WineFox.Cards.Uncommon;
using STS2_WineFox.Character;
using STS2_WineFox.Enchantments;
using STS2_WineFox.Epoch;
using STS2_WineFox.Potions;
using STS2_WineFox.Powers;
using STS2_WineFox.Relics;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Content.Descriptors
{
    /// <summary>
    ///     All declarative registrations for this mod: ModelDb content (split by kind), keywords (separate registry), and
    ///     pack steps (timeline + unlocks). Apply order should be content → keywords → pack (unlocks need models registered).
    /// </summary>
    internal static class WineFoxContentManifest
    {
        private const string Root = "res://STS2_WineFox";

        private static readonly IContentRegistrationEntry[] CharacterAndSharedPoolEntries =
        [
            new CharacterRegistrationEntry<WineFox>(),
            new SharedCardPoolRegistrationEntry<WineFoxTokenCardPool>(),
        ];

        private static readonly IContentRegistrationEntry[] CharacterCardPoolEntries =
        [
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
            new CardRegistrationEntry<WineFoxCardPool, Forging>(),
            new CardRegistrationEntry<WineFoxCardPool, EquivalentExchange>(),
            new CardRegistrationEntry<WineFoxCardPool, QuickShelter>(),
            new CardRegistrationEntry<WineFoxCardPool, BlueprintPrinting>(),
            new CardRegistrationEntry<WineFoxCardPool, StressResponse>(),
            new CardRegistrationEntry<WineFoxCardPool, MassProduction>(),
            new CardRegistrationEntry<WineFoxCardPool, WirelessTerminal>(),
            new CardRegistrationEntry<WineFoxCardPool, SnowBallOverwhelming>(),
            new CardRegistrationEntry<WineFoxCardPool, HellGift>(),
        ];

        private static readonly IContentRegistrationEntry[] TokenCardPoolEntries =
        [
            new CardRegistrationEntry<WineFoxTokenCardPool, Nothing>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, WoodenPickaxe>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, WoodenSword>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, WoodenArmor>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, StonePickaxe>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, StoneSword>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, StoneArmor>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, IronPickaxe>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, IronSword>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, IronArmor>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, DiamondSword>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, DiamondArmor>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, WorkWork>(),
            new CardRegistrationEntry<WineFoxTokenCardPool, GoldenSword>(),
        ];

        private static readonly IContentRegistrationEntry[] RelicAndOrobasEntries =
        [
            new RelicRegistrationEntry<WineFoxRelicPool, HandCrank>(),
            new RelicRegistrationEntry<WineFoxRelicPool, MaidBackpack>(),
            new RelicRegistrationEntry<WineFoxRelicPool, Deployer>(),
            new TouchOfOrobasRefinementRegistrationEntry<HandCrank, Deployer>(),
        ];

        private static readonly IContentRegistrationEntry[] PowerEntries =
        [
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
            new PowerRegistrationEntry<BurningPower>(),
            new PowerRegistrationEntry<GoldenSwordPower>(),
            new PowerRegistrationEntry<MassProductionPower>(),
            new PowerRegistrationEntry<SnowBallOverwhelmingPower>(),
        ];

        private static readonly IContentRegistrationEntry[] PotionEntries =
        [
            // new PotionRegistrationEntry<WineFoxPotionPool,GoldenApple>()
        ];
        
        private static readonly IContentRegistrationEntry[] EnchantmentEntries =
        [
            new EnchantmentRegistrationEntry<FireAspect>(),
            new EnchantmentRegistrationEntry<SweepingEdge>(),
        ];

        private static readonly Type[] EpochCardPackTypes =
        [
            typeof(AlterPath),
            typeof(EnmergencyRepair),
            typeof(IronZombie),
            typeof(LightAssault),
            typeof(MechanicalSaw),
            typeof(PlantTrees),
            typeof(CrushingWheel),
            typeof(FullAttack),
            typeof(MechanicalDrill),
            typeof(PowerUp),
            typeof(MiningGems),
            typeof(SteamEngine),
        ];

        private static readonly Type[] EpochAct1CardTypes =
        [
            typeof(EasyPeasy),
            typeof(FoxBite),
            typeof(ShieldAttack),
        ];

        private static readonly Type[] EpochAct3CardTypes =
        [
            typeof(SpinningHand),
            typeof(VacantDomain),
            typeof(MaidSupport),
        ];

        private static readonly Type[] EpochVictoryCardTypes =
        [
            typeof(Traditionalist),
            typeof(RecordPlayer),
            typeof(WorkbenchBackpack),
        ];

        private static readonly Type[] EpochEliteCardTypes =
        [
            typeof(AllItem),
            typeof(LessHoliday),
            typeof(Alternator),
        ];

        private static readonly Type[] EpochBossCardTypes =
        [
            typeof(CobblestoneGenerator),
            typeof(NetheritePickaxe),
            typeof(RiclearPowerPlant),
        ];

        private static readonly Type[] EpochAscensionOneCardTypes =
        [
            typeof(SlashBladeWood),
            typeof(DripstoneTrap),
            typeof(Forging),
        ];

        private static readonly Lazy<IReadOnlyList<IModContentPackEntry>> PackEntriesLazy = new(CreatePackEntries);

        /// <summary>
        ///     Flat list for <see cref="ModContentPackBuilder.ContentManifest" /> (order = registration order).
        /// </summary>
        public static IReadOnlyList<IContentRegistrationEntry> ContentEntries { get; } =
            Concat(
                CharacterAndSharedPoolEntries,
                CharacterCardPoolEntries,
                TokenCardPoolEntries,
                RelicAndOrobasEntries,
                PowerEntries,
                PotionEntries,
                EnchantmentEntries);

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

        /// <summary>
        ///     Timeline: <see cref="TimelineColumnPackEntry{TStory}" /> for column order and per-epoch unlock bindings;
        ///     explicit card type arrays live at the bottom of this file (<c>Epoch*Types</c>).
        /// </summary>
        public static IReadOnlyList<IModContentPackEntry> PackEntries => PackEntriesLazy.Value;

        public static IReadOnlyList<T> Concat<T>(params IEnumerable<T>[] chunks)
        {
            return chunks.SelectMany(static c => c).ToArray();
        }

        // TimelineColumn epoch-slot API cheat sheet (cards / relics / potions). Uncomment and wire types + ContentEntries as needed.
        //
        // Cards
        //   · Whole pool behind character epoch: RequireAllCardsInPool<WineFoxCardPool>() (used on character epoch above)
        //   · Explicit list + pack-declared unlock data: Cards(EpochXxxCardTypes)
        //   · Whole pool into ModEpochGatedContentRegistry: CardsFromPool<WineFoxCardPool>()
        // Relics
        //   · Whole pool, RequireEpoch only: RequireAllRelicsInPool<WineFoxRelicPool>()
        //   · Explicit: Relics(types); pool + registry: RelicsFromPool<WineFoxRelicPool>() (Act2 above)
        // Potions (register potions first: SharedPotionPool + PotionRegistrationEntry<TPool,TPotion> in ContentEntries)
        //   · Whole pool gated: .Epoch<WineFoxSomeEpoch>(e => e.RequireAllPotionsInPool<WineFoxPotionPool>())
        //   · Explicit types: .Epoch<WineFoxSomeEpoch>(e => e.Potions(new[] { typeof(MyPotionA), typeof(MyPotionB) }))
        //   · Timeline potion splash: epoch subclasses PotionUnlockEpochTemplate + PotionTypes, same types as Potions/RequireEpoch
        // Alternatively: ModContentPackBuilder chain .RequireEpoch<TPotion, TEpoch>() per potion (outside TimelineColumn)
        private static IReadOnlyList<IModContentPackEntry> CreatePackEntries()
        {
            return
            [
                new TimelineColumnPackEntry<WineFoxModStory>(c => c
                    .Epoch<WineFoxCharacterEpoch>(e => e.RequireAllCardsInPool<WineFoxCardPool>())
                    .Epoch<WineFoxCardEpoch>(e => e.Cards(EpochCardPackTypes))
                    .Epoch<WineFoxAct1Epoch>(e => e.Cards(EpochAct1CardTypes))
                    .Epoch<WineFoxAct2Epoch>(e => e.RelicsFromPool<WineFoxRelicPool>())
                    .Epoch<WineFoxAct3Epoch>(e => e.Cards(EpochAct3CardTypes))
                    .Epoch<WineFoxVictoryEpoch>(e => e.Cards(EpochVictoryCardTypes))
                    .Epoch<WineFoxEliteEpoch>(e => e.Cards(EpochEliteCardTypes))
                    .Epoch<WineFoxBossEpoch>(e => e.Cards(EpochBossCardTypes))
                    .Epoch<WineFoxAscensionOneEpoch>(e => e.Cards(EpochAscensionOneCardTypes))
                    .RegisterStory()),
                new RequireEpochPackEntry<WineFox, WineFoxCharacterEpoch>(),
                new UnlockEpochAfterWinAsPackEntry<Ironclad, WineFoxCharacterEpoch>(),
                new UnlockEpochAfterRunAsPackEntry<WineFox, WineFoxCardEpoch>(),
                new UnlockEpochAfterWinAsPackEntry<WineFox, WineFoxVictoryEpoch>(),
                new UnlockEpochAfterEliteVictoriesPackEntry<WineFox, WineFoxEliteEpoch>(),
                new UnlockEpochAfterBossVictoriesPackEntry<WineFox, WineFoxBossEpoch>(),
                new UnlockEpochAfterAscensionOneWinPackEntry<WineFox, WineFoxAscensionOneEpoch>(),
                new RevealAscensionAfterEpochPackEntry<WineFox, WineFoxVictoryEpoch>(),
            ];
        }
    }
}
