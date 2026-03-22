namespace STS2_WineFox;

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
        public const string CharacterSelectBgScene = ScenesRoot + "/screens/char_select/char_select_bg_wine_fox.tscn";

        public const string CharacterIcon = Root + "/winefox/character_icon_wine_fox.png";
        public const string CharacterIconOutline = Root + "/winefox/character_icon_wine_fox_outline.png";
        public const string CharacterSelectIcon = Root + "/packed/character_select/char_select_wine_fox.png";
        public const string CustomEnergyCounterPath = Root + "/ui/energy_counters/winefox_energy_counter.tscn";
        public const string CharacterSelectLockedIcon =
            Root + "/winefox/char_select_wine_fox_locked.png";

        public const string MapMarker = Root + "/packed/map/icons/map_marker_wine_fox.png";

        public const string HandCrankRelicIcon = Root + "/relics/sts2_wine_fox_relic_hand_crank.png";

        public const string DefaultTransitionMaterial = "res://materials/transitions/silent_transition_mat.tres";
        public const string DefaultTrailScene = "res://scenes/vfx/card_trail_silent.tscn";
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
        public const string CardWoodenSword    = Root + "/cards/card_woodensword.png";
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