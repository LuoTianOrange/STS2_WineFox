using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Character
{
    // 遗物池子
    public class WineFoxRelicPool : TypeListRelicPoolModel
    {
        public override string EnergyColorName => Const.EnergyColorName;
        public override string? BigEnergyIconPath => Const.Paths.EnergyIconCake;
        public override string? TextEnergyIconPath => Const.Paths.EnergyIconCake;
        public override Color LabOutlineColor => WineFox.Color;
    }
}
