using BaseLib.Abstracts;
using Godot;

namespace STS2_WineFox.Character;

// 遗物池子
public class WineFoxRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => "winefox";
    public override Color LabOutlineColor => WineFox.Color;
}