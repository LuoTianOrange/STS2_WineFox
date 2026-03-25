namespace STS2_WineFox
{
    public static class Const
    {
        public const string ModId = "STS2-WineFox";
        public const string Name = "WineFox";
        public const string Version = "0.1.0";

        public const string EnergyColorName = "winefox";

        public static class Paths
        {
            public const string Root = "res://STS2_WineFox";
            public const string ScenesRoot = Root + "/scenes";

            public const string EnergyIconCake = Root + "/winefox/cake.png";
            public const string CharacterVisualsScene = ScenesRoot + "/winefox/winefox.tscn";
            public const string CharacterIconScene = ScenesRoot + "/ui/character_icons/wine_fox_icon.tscn";
            public const string CharacterSelectBgScene = ScenesRoot + "/char_select/char_select_bg_wine_fox.tscn";
            public const string CharacterRestSiteAnimScene = ScenesRoot + "/rest_site/winefox_rest_site.tscn";
            public const string CharacterIcon = Root + "/winefox/character_icon_wine_fox.png";
            public const string CharacterIconOutline = Root + "/winefox/character_icon_wine_fox_outline.png";
            public const string CharacterSelectIcon = Root + "/packed/character_select/char_select_wine_fox.png";
            public const string CustomEnergyCounterPath = Root + "/ui/energy_counters/winefox_energy_counter.tscn";

            public const string CharacterSelectLockedIcon =
                Root + "/winefox/char_select_wine_fox_locked.png";

            public const string MapMarker = Root + "/packed/map/icons/map_marker_wine_fox.png";

            public const string DefaultTransitionMaterial = "res://materials/transitions/silent_transition_mat.tres";

            public const string DefaultTrailScene = "res://scenes/vfx/card_trail_silent.tscn";
            //Relics
            public const string HandCrankRelicIcon = Root + "/relics/relic_hand_crank.png";
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

            //Card
            public const string CardStonePickaxe = Root + "/cards/card_stone_pickaxe.png";
            public const string CardWineFoxDefend = Root + "/cards/card_winefoxdefend.png";
            public const string CardWineFoxStrike = Root + "/cards/card_winefoxstrike.png";
            public const string CardBaseCraft = Root + "/cards/card_basecraft.png";
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
            public const string CardAlterPath =  Root + "/cards/card_alterpath.png";
            public const string CardStoneArmor = Root + "/cards/card_stonearmor.png";
            public const string CardPowerUp = Root + "/cards/card_powerup.png";
            public const string CardIronZombie = Root + "/cards/card_ironzombie.png";
            public const string CardCrushingWheel = Root + "/cards/card_crushingwheel.png";
            public const string CardEnmergencyRepair = Root + "/cards/card_test.png";
            public const string CardLightAssault = Root + "/cards/card_test.png";
            
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
