using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models.Characters;
using STS2_WineFox.Content.Descriptors;
using STS2_WineFox.Epoch;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;

namespace STS2_WineFox.Character
{
    [RegisterCharacter]
    [RequireEpoch(typeof(WineFoxCharacterEpoch))]
    [UnlockEpochAfterRunAs(typeof(WineFoxCardEpoch))]
    [UnlockEpochAfterWinAs(typeof(WineFoxVictoryEpoch))]
    [UnlockEpochAfterEliteVictories(typeof(WineFoxEliteEpoch))]
    [UnlockEpochAfterBossVictories(typeof(WineFoxBossEpoch))]
    [UnlockEpochAfterAscensionOneWin(typeof(WineFoxAscensionOneEpoch))]
    [RevealAscensionAfterEpoch(typeof(WineFoxVictoryEpoch))]
    public class WineFox : ModCharacterTemplate<WineFoxCardPool, WineFoxRelicPool, WineFoxPotionPool>
    {
        public static readonly Color Color = new("ffaf50");

        public override Color NameColor => Color;
        public override Color MapDrawingColor => Colors.Orange;
        public override int StartingHp => 80;
        public override int StartingGold => 99;
        public override CharacterGender Gender => CharacterGender.Neutral;
        public override CharacterAssetProfile AssetProfile => WineFoxCharacterAssets.Profile;
        public override float AttackAnimDelay => 0.15f;
        public override float CastAnimDelay => 0.25f;

        protected override Type UnlocksAfterRunAsType => typeof(Ironclad);

        public override List<string> GetArchitectAttackVfx()
        {
            return
            [
                "vfx/vfx_attack_blunt",
                "vfx/vfx_heavy_blunt",
                "vfx/vfx_attack_slash",
            ];
        }
    }
}
