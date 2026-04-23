using Godot;
using STS2RitsuLib.Scaffolding.Characters;
using STS2RitsuLib.Scaffolding.Visuals;

namespace STS2_WineFox.Content.Descriptors
{
    internal static class WineFoxCharacterAssets
    {
        private static readonly CharacterAssetProfile BaseProfile = CharacterAssetProfiles.Ironclad();

        internal static CharacterAssetProfile Profile { get; } = BaseProfile
            .WithScenes(BaseProfile.Scenes! with
            {
                VisualsPath = Const.Paths.CharacterVisualsScene,
                EnergyCounterPath = Const.Paths.CustomEnergyCounterPath,
                RestSiteAnimPath = Const.Paths.CharacterRestSiteAnimScene,
                MerchantAnimPath = Const.Paths.CharacterMerchantAnimScene,
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
            .WithMultiplayer(new(
                Const.Paths.ArmPointingTexturePath,
                Const.Paths.ArmRockTexturePath,
                Const.Paths.ArmPaperTexturePath,
                Const.Paths.ArmScissorsTexturePath
            ))
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
            .WithVisualCues(
                ModVisualCues.CueSet()
                    .Single("die", Const.Paths.CharacterDeathPortrait)
                    // 可选：防止 revive 后仍停在死亡图
                    .Single("revive", Const.Paths.CharacterAlivePortrait)
                    .Single("idle",   Const.Paths.CharacterAlivePortrait)
                    .Build())
            .WithAudio(new(
                Const.Audio.CharacterSelect,
                Const.Audio.CharacterTransition,
                Const.Audio.Attack,
                Const.Audio.Cast,
                Const.Audio.Death));
        
    }
}
