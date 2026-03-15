using BaseLib.Abstracts;
using Godot;

namespace STS2_WineFox.Character;

// 卡组池子
public partial class WineFoxCardPool : CustomCardPoolModel
{
    public override string Title => "winefox";
    
    public override string EnergyColorName => "winefox";
    
    public override float H => 0.099f;
    public override float S => 0.571f;
    public override float V => 0.824f;
    
    public override Color DeckEntryCardColor => new("d2a15a");
    public override Color EnergyOutlineColor => new("fffd");
    public override bool IsColorless => false;
    
}