using Godot;
using STS2_WineFox.Cards.Basic;
using STS2_WineFox.Cards.Common;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Token;
using STS2_WineFox.Cards.Uncommon;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Character
{
    // 临时复用 Silent 的完整奖励池，避免在自定义非基础卡不足时奖励生成失败。
    public class WineFoxCardPool : TypeListCardPoolModel
    {
        public override string Title => Const.EnergyColorName;

        public override string EnergyColorName => Const.EnergyColorName;
        public override string? BigEnergyIconPath => Const.Paths.EnergyIconCake;
        public override string? TextEnergyIconPath => Const.Paths.EnergyIconCake;
        public override string CardFrameMaterialPath => "card_frame_orange";

        public override Color DeckEntryCardColor => new("d2a15a");
        public override Color EnergyOutlineColor => new("8d4b24");
        public override bool IsColorless => false;

        protected override IEnumerable<Type> CardTypes =>
        [
            typeof(WineFoxStrike),
            typeof(WineFoxDefend),
            typeof(BasicMine),
            typeof(BaseCraft),
            typeof(FullAttack),
            typeof(MiningGems),
            typeof(PlantTrees),
            typeof(SteamEngine),
            typeof(MechanicalDrill),
            typeof(MechanicalSaw),
            typeof(AlterPath),
            typeof(PowerUp),
            typeof(IronZombie),
            typeof(CrushingWheel),
            typeof(EnmergencyRepair),
            typeof(LightAssault),
            
            //Token
            typeof(StonePickaxe),
            typeof(StoneSword),
            typeof(IronArmor),
            typeof(WoodenSword),
            typeof(IronPickaxe),
            typeof(DiamondSword),
            typeof(IronSword),
            typeof(StoneArmor)
        ];
    }
}
