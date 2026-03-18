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
                VisualsPath = Const.Paths.CharacterVisualsScene,
            })
            .WithUi(new(
                Const.Paths.CharacterIcon,
                Const.Paths.CharacterIconOutline,
                Const.Paths.CharacterIconScene,
                Const.Paths.CharacterSelectBgScene,
                Const.Paths.CharacterSelectIcon,
                Const.Paths.CharacterSelectLockedIcon,
                Const.Paths.DefaultTransitionMaterial,
                Const.Paths.MapMarker))
            .WithVfx(new(
                Const.Paths.DefaultTrailScene,
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
                Const.Audio.CharacterSelect,
                Const.Audio.CharacterTransition,
                Const.Audio.Attack,
                Const.Audio.Cast,
                Const.Audio.Death));
    }
}
