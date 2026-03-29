using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using STS2_WineFox.Cards.Basic;
using STS2_WineFox.Content.Descriptors;
using STS2_WineFox.Relics;
using STS2RitsuLib.Scaffolding.Characters;

namespace STS2_WineFox.Character
{
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

        protected override IEnumerable<Type> StartingDeckTypes =>
        [
            typeof(WineFoxStrike),
            typeof(WineFoxStrike),
            typeof(WineFoxStrike),
            typeof(WineFoxStrike),
            typeof(WineFoxDefend),
            typeof(WineFoxDefend),
            typeof(WineFoxDefend),
            typeof(WineFoxDefend),
            typeof(BasicMine),
            typeof(BasicCraft),
        ];

        protected override IEnumerable<Type> StartingRelicTypes => [typeof(HandCrank)];

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
