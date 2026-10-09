using Godot;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Character
{
    public class MagicWineFoxCardPool : TypeListCardPoolModel
    {
        public override string Title => Const.MagicEnergyColorName;

        public override string EnergyColorName => Const.MagicEnergyColorName;
        public override string? BigEnergyIconPath => Const.Paths.MagicEnergyIcon;
        public override string? TextEnergyIconPath => Const.Paths.MagicEnergyIcon;
        public override string CardFrameMaterialPath => "card_frame_orange";

        /// <summary>
        ///     边框贴图是画好的成品图，不能再被卡池色染色（默认材质会把边框趋向卡池色）。
        ///     恒等调制的 HSV 材质保留原版着色管线但不改颜色。
        /// </summary>
        public override Material? PoolFrameMaterial =>
            MaterialUtils.CreateUnmodulatedHsvShaderMaterial();

        public override Color DeckEntryCardColor => new("b66bff");
        public override Color EnergyOutlineColor => new("5f2f91");
        public override bool IsColorless => false;
    }
}
