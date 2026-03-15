using BaseLib.Abstracts;
using Godot;

namespace STS2_WineFox.Character;

// 药水池子
public class WineFoxPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => "winefox";
    public override Color LabOutlineColor => WineFox.Color;
}