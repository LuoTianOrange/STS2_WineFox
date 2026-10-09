using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Character
{
    // 遗物池子
    public class MagicWineFoxRelicPool : TypeListRelicPoolModel
    {
        public override string EnergyColorName => Const.MagicEnergyColorName;
        public override string? BigEnergyIconPath => Const.Paths.MagicEnergyIcon;
        public override string? TextEnergyIconPath => Const.Paths.MagicEnergyIcon;
        public override Color LabOutlineColor => MagicWineFox.Color;
    }
}
