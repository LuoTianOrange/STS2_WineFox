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
            public const string CharacterSelectBgScene = ScenesRoot + "/screens/char_select/char_select_bg_wine_fox.tscn";

            public const string CharacterIcon = Root + "/ui/top_panel/character_icon_wine_fox.png";
            public const string CharacterIconOutline = Root + "/ui/top_panel/character_icon_wine_fox_outline.png";
            public const string CharacterSelectIcon = Root + "/packed/character_select/char_select_wine_fox.png";
            public const string CharacterSelectLockedIcon = Root + "/packed/character_select/char_select_wine_fox_locked.png";
            public const string MapMarker = Root + "/packed/map/icons/map_marker_wine_fox.png";

            public const string HandCrankRelicIcon = Root + "/relics/sts2_wine_fox_relic_hand_crank.png";

            public const string DefaultTransitionMaterial = "res://materials/transitions/silent_transition_mat.tres";
            public const string DefaultTrailScene = "res://scenes/vfx/card_trail_silent.tscn";
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
