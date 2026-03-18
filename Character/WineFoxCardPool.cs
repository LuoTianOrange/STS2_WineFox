using Godot;
using STS2_WineFox.Cards.Basic;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Character
{
    // 临时复用 Silent 的完整奖励池，避免在自定义非基础卡不足时奖励生成失败。
    public class WineFoxCardPool : TypeListCardPoolModel
    {
        public override string Title => Const.EnergyColorName;

        public override string EnergyColorName => Const.EnergyColorName;
        public override string? TextEnergyIconPath => Const.Paths.EnergyIconCake;
        public override string CardFrameMaterialPath => "card_frame_colorless";

        public override Color DeckEntryCardColor => new("d2a15a");
        public override Color EnergyOutlineColor => new("8d4b24");
        public override bool IsColorless => false;

        protected override IEnumerable<Type> CardTypes =>
        [
            typeof(WineFoxStrike),
            typeof(WineFoxDefend),
            typeof(BasicMine),
        ];
    }
}
