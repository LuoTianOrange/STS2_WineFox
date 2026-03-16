using Godot;
using STS2RitsuLib.Scaffolding.Characters;

namespace STS2_WineFox.Content.Descriptors
{
    internal static class WineFoxCharacterAssets
    {
        private static readonly CharacterAssetProfile BaseProfile = CharacterAssetProfiles.Silent();

        internal static CharacterAssetProfile Profile { get; } = BaseProfile
            .WithScenes(BaseProfile.Scenes! with
            {
                VisualsPath = "res://scenes/winefox/winefox.tscn",
            })
            .WithUi(new(
                "res://images/ui/top_panel/character_icon_wine_fox.png",
                "res://images/ui/top_panel/character_icon_wine_fox_outline.png",
                "res://scenes/ui/character_icons/wine_fox_icon.tscn",
                "res://scenes/screens/char_select/char_select_bg_wine_fox.tscn",
                "res://images/packed/character_select/char_select_wine_fox.png",
                "res://images/packed/character_select/char_select_wine_fox_locked.png",
                "res://materials/transitions/silent_transition_mat.tres",
                "res://images/packed/map/icons/map_marker_wine_fox.png"))
            .WithVfx(new(
                "res://scenes/vfx/card_trail_silent.tscn",
                new(
                    new Color(0.9529412f, 0.5294118f, 0.7607843f, 0.55f),
                    82f,
                    new Color(1f, 0.8666667f, 0.9372549f, 0.8f),
                    42f,
                    new Color(1f, 0.7529412f, 0.8901961f, 0.85f),
                    new Color(1f, 0.9333333f, 0.9686275f, 0.95f),
                    new Color(1f, 0.7411765f, 0.8901961f, 0.55f),
                    new Vector2(1.05f, 1.0f),
                    new Color(1f, 0.9568627f, 0.9843137f, 0.9f),
                    new Vector2(0.82f, 0.82f))))
            .WithAudio(new(
                "event:/sfx/characters/silent/silent_select",
                "event:/sfx/ui/wipe_silent",
                "event:/sfx/characters/silent/silent_attack",
                "event:/sfx/characters/silent/silent_cast",
                "event:/sfx/characters/silent/silent_die"));
    }
}
