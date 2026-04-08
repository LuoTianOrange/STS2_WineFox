namespace STS2_WineFox
{
    public static class Const
    {
        public const string ModId = "STS2-WineFox";
        public const string Name = "WineFox";
        public const string Version = "0.1.0";

        /// <summary>
        ///     When true, WineFox skips <c>RequireEpoch</c> gating for this mod’s models (full pool / character access without
        ///     timeline progress).
        /// </summary>
        public const bool IgnoreEpochRequirements = true;

        public const string EnergyColorName = "winefox";

        public static class Paths
        {
            public const string Root = "res://STS2_WineFox";
            public const string ScenesRoot = Root + "/scenes";

            public const string EnergyIconCake = Root + "/winefox/winefox_energy_icon.png";
            public const string CharacterVisualsScene = ScenesRoot + "/winefox/winefox.tscn";
            public const string CharacterIconScene = ScenesRoot + "/ui/character_icons/wine_fox_icon.tscn";
            public const string CharacterSelectBgScene = ScenesRoot + "/char_select/char_select_bg_wine_fox.tscn";
            public const string CharacterRestSiteAnimScene = ScenesRoot + "/rest_site/winefox_rest_site.tscn";
            public const string CharacterIcon = Root + "/winefox/character_icon_wine_fox.png";
            public const string CharacterIconOutline = Root + "/winefox/character_icon_wine_fox_outline.png";
            public const string CharacterSelectIcon = Root + "/packed/character_select/char_select_wine_fox.png";
            public const string CustomEnergyCounterPath = Root + "/ui/energy_counters/winefox_energy_counter.tscn";
            public const string CharacterMerchantAnimScene = ScenesRoot + "/shop/winefox_shop.tscn";
            public const string ArmPointingTexturePath = Root + "/winefox/arm/winefox_point.png";
            public const string ArmRockTexturePath = Root + "/winefox/arm/winefox_rock.png";
            public const string ArmPaperTexturePath = Root + "/winefox/arm/winefox_paper.png";
            public const string ArmScissorsTexturePath = Root + "/winefox/arm/winefox_scissors.png";


            public const string CharacterSelectLockedIcon =
                Root + "/winefox/char_select_wine_fox_locked.png";

            public const string MapMarker = Root + "/packed/map/icons/map_marker_wine_fox.png";

            public const string DefaultTransitionMaterial = "res://materials/transitions/silent_transition_mat.tres";

            public const string DefaultTrailScene = "res://scenes/vfx/card_trail_silent.tscn";

            //Relics
            public const string HandCrankRelicIcon = Root + "/relics/relic_hand_crank.png";
            public const string DeployerRelicIcon = Root + "/relics/relic_deployer.png";
            public const string MaidBackpackRelicIcon = Root + "/relics/relic_maid_backpack.png";

            //Power
            public const string WoodPowerIcon = Root + "/powers/wood_power.png";
            public const string WoodPowerBigIcon = Root + "/powers/wood_power.png";
            public const string PlantPowerIcon = Root + "/powers/plant_power.png";
            public const string StonePowerIcon = Root + "/powers/stone_power.png";
            public const string StonePowerBigIcon = Root + "/powers/stone_power.png";
            public const string StressPowerIcon = Root + "/powers/stress_power.png";
            public const string StressPowerBigIcon = Root + "/powers/stress_power.png";
            public const string DiggingPowerIcon = Root + "/powers/digging.png";
            public const string SteamPowerIcon = Root + "/powers/steam_power.png";
            public const string SteamPowerBigIcon = Root + "/powers/steam_power.png";
            public const string StoneSwordPowerIcon = Root + "/powers/stone_sword_power.png";
            public const string IronPowerIcon = Root + "/powers/iron_power.png";
            public const string WoodenSwordPowerIcon = Root + "/powers/wooden_sword_power.png";
            public const string IronPickaxePowerIcon = Root + "/powers/iron_pickaxe_power.png";
            public const string DiamondPowerIcon = Root + "/powers/diamond_power.png";
            public const string DiamondSwordPowerIcon = Root + "/powers/diamond_sword_power.png";
            public const string StoneArmorPowerIcon = Root + "/powers/stone_armor_power.png";
            public const string IronSwordPowerIcon = Root + "/powers/iron_sword_power.png";
            public const string RepairPowerIcon = Root + "/powers/repair_power.png";
            public const string RadiationLeakIcon = Root + "/powers/radiation_leak_power.png";
            public const string EasyPeasyPowerIcon = Root + "/powers/easy_peasy_power.png";
            public const string NetheritePickaxePowerIcon = Root + "/powers/netherite_pickaxe_power.png";
            public const string BrushStoneFormPowerIcon = Root + "/powers/brush_stone_form_power.png";
            public const string SlashBladeWoodPowerIcon = Root + "/powers/slash_blade_wood_power.png";
            public const string ProductionDocumentPowerIcon = Root + "/powers/production_document_power.png";
            public const string DiamondArmorPowerIcon = Root + "/powers/diamond_armor_power.png";
            public const string MassProductionPowerIcon = Root + "/powers/mass_production_power.png";
            public const string GoldenSwordPowerIcon = Root + "/powers/golden_sword_power.png";
            public const string SnowBallOverwhelmingPowerIcon = Root + "/powers/snow_ball_overwhelming_power.png";
            public const string DiamondPickaxePowerIcon = Root + "/powers/diamond_pickaxe_power.png";
            public const string GoldenPickaxePowerIcon = Root + "/powers/golden_pickaxe_power.png";
            public const string ShieldCooldownPowerIcon = Root + "/powers/shield_cooldown_power.png";
            public const string ShieldDexPowerIcon = Root + "/powers/shield_dex_power.png";
            public const string AutoCrafterPowerIcon = Root + "/powers/auto_crafter_power.png";
            public const string IronArmorPowerIcon = Root + "/powers/iron_armor_power.png";
            public const string NetheriteChestPlatePowerIcon = Root + "/powers/netherite_chest_plate_power.png";
            public const string HighlyFocusedPowerIcon = Root + "/powers/highly_focused_power.png";

            public const string BurningIcon = Root + "/powers/burning_power.png";

            //Card
            public const string CardStonePickaxe = Root + "/cards/card_stone_pickaxe.png";
            public const string CardWineFoxDefend = Root + "/cards/card_winefoxdefend.png";
            public const string CardWineFoxStrike = Root + "/cards/card_winefoxstrike.png";
            public const string CardBasicCraft = Root + "/cards/card_basiccraft.png";
            public const string CardBaseMine = Root + "/cards/card_basemine.png";
            public const string CardFullAttack = Root + "/cards/card_fullattack.png";
            public const string CardMiningGems = Root + "/cards/card_mininggems.png";
            public const string CardPlantTrees = Root + "/cards/card_planttrees.png";
            public const string CardSteamEngine = Root + "/cards/card_steamengine.png";
            public const string CardStoneSword = Root + "/cards/card_stonesword.png";
            public const string CardIronArmor = Root + "/cards/card_ironarmor.png";
            public const string CardMechanicalDrill = Root + "/cards/card_mechanicaldrill.png";
            public const string CardWoodenSword = Root + "/cards/card_woodensword.png";
            public const string CardIronPickaxe = Root + "/cards/card_ironpickaxe.png";
            public const string CardDiamondSword = Root + "/cards/card_diamondsword.png";
            public const string CardMechanicalSaw = Root + "/cards/card_mechanicalsaw.png";
            public const string CardIronSword = Root + "/cards/card_ironsword.png";
            public const string CardAlterPath = Root + "/cards/card_alterpath.png";
            public const string CardStoneArmor = Root + "/cards/card_stonearmor.png";
            public const string CardPowerUp = Root + "/cards/card_powerup.png";
            public const string CardIronZombie = Root + "/cards/card_ironzombie.png";
            public const string CardCrushingWheel = Root + "/cards/card_crushingwheel.png";
            public const string CardEnmergencyRepair = Root + "/cards/card_enmergency_repair.png";
            public const string CardLightAssault = Root + "/cards/card_test.png";
            public const string CardEasyPeasy = Root + "/cards/card_easy_peasy.png";
            public const string CardAllItem = Root + "/cards/card_allitem.png";
            public const string CardRiclearPowerPlant = Root + "/cards/card_riclear_power_plant.png";
            public const string CardAlternator = Root + "/cards/card_alternator.png";
            public const string CardNothing = Root + "/cards/card_test.png";
            public const string CardWoodenPickaxe = Root + "/cards/card_woodenpickaxe.png";
            public const string CardWoodenArmor = Root + "/cards/card_woodenarmor.png";
            public const string CardNetheritePickaxe = Root + "/cards/card_netherite_pickaxe.png";
            public const string CardCobblestoneGenerator = Root + "/cards/card_test.png";
            public const string CardShieldAttack = Root + "/cards/card_shield_attack.png";
            public const string CardSpinningHand = Root + "/cards/card_test.png";
            public const string CardProductionDocument = Root + "/cards/card_test.png";
            public const string CardMilk = Root + "/cards/card_milk.png";
            public const string CardVacantDomain = Root + "/cards/card_vacantdomain.png";
            public const string CardRecordPlayer = Root + "/cards/card_record_player.png";
            public const string CardPressWToThink = Root + "/cards/card_press_w_to_think.png";
            public const string CardBackupCrafting = Root + "/cards/card_backup_crafting.png";
            public const string CardTicTacToeGrid = Root + "/cards/card_tic_tac_toe_grid.png";
            public const string CardWorkWork = Root + "/cards/card_work_work.png";
            public const string CardLessHoliday = Root + "/cards/card_less_holiday.png";
            public const string CardTraditionalist = Root + "/cards/card_traditionalist.png";
            public const string CardSlashBladeWood = Root + "/cards/card_slash_blade_wood.png";
            public const string CardDripstoneTrap = Root + "/cards/card_dripstone_trap.png";
            public const string CardPreProcessing = Root + "/cards/card_pre_processing.png";
            public const string CardWorkbenchBackpack = Root + "/cards/card_workbench_backpack.png";
            public const string CardRegroup = Root + "/cards/card_regroup.png";
            public const string CardChangeEquipment = Root + "/cards/card_change_equipment.png";
            public const string CardDiamondArmor = Root + "/cards/card_diamond_armor.png";
            public const string CardFoxBite = Root + "/cards/card_fox_bite.png";
            public const string CardMaidSupport = Root + "/cards/card_maid_support.png";
            public const string CardForging = Root + "/cards/card_forgeing.png";
            public const string CardEquivalentExchange = Root + "/cards/card_equivalent_exchange.png";
            public const string CardQuickShelter = Root + "/cards/card_quick_shelter.png";
            public const string CardBlueprintPrinting = Root + "/cards/card_blueprint_printing.png";
            public const string CardStressResponse = Root + "/cards/card_stress_response.png";
            public const string CardMassProduction = Root + "/cards/card_mass_production.png";
            public const string CardWirelessTerminal = Root + "/cards/card_wireless_terminal.png";
            public const string CardGoldenSword = Root + "/cards/card_golden_sword.png";
            public const string CardHellGift = Root + "/cards/card_hell_gift.png";
            public const string CardSnowBallOverwhelming = Root + "/cards/card_snow_ball_overwhelming.png";
            public const string CardDiamondPickaxe = Root + "/cards/card_diamond_pickaxe.png";
            public const string CardGoldenPickaxe = Root + "/cards/card_golden_pickaxe.png";
            public const string CardBucket = Root + "/cards/card_bucket.png";
            public const string CardSteelChamber = Root + "/cards/card_steel_chamber.png";
            public const string CardNoMoreFalchion = Root + "/cards/card_no_more_falchion.png";
            public const string CardShield = Root + "/cards/card_shield.png";
            public const string CardAutoCrafter = Root + "/cards/card_auto_crafter.png";
            public const string CardPaybackTime = Root + "/cards/card_payback_time.png";
            public const string CardKaleidoscopePot = Root + "/cards/card_kaleidoscope_pot.png";
            public const string CardHammerStrike = Root + "/cards/card_hammer_strike.png";
            public const string CardQuickAttack = Root + "/cards/card_quick_attack.png";
            public const string CardNetheriteChestPlate = Root + "/cards/card_netherite_chest_plate.png";
            public const string CardHighlyFocused = Root + "/cards/card_highly_focused.png";
            public const string CardSweetDream = Root + "/cards/card_sweet_dream.png";


            //Enchantments
            public const string EnchantmentFireAspectIcon = Root + "/enchantments/fire_aspect_icon.png";
            public const string EnchantmentSweepingEdgeIcon = Root + "/enchantments/sweeping_edge_icon.png";
        }

        public static class Audio
        {
            public const string CharacterSelect = "event:/sfx/characters/silent/silent_select";
            public const string CharacterTransition = "event:/sfx/ui/wipe_silent";
            public const string Attack = "event:/sfx/characters/silent/silent_attack";
            public const string Cast = "event:/sfx/characters/silent/silent_cast";
            public const string Death = "event:/sfx/characters/silent/silent_die";
        }
    }
}
