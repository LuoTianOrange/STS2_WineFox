using Godot;
using MegaCrit.Sts2.Core.Localization;

namespace STS2_WineFox.Scripts.Effects;

public partial class VfxHoldNToPonder : Node2D
{
    [Export] public Sprite2D NodeBG;
    [Export] public Sprite2D NodeText;
    
    [Export] public Texture2D BGTexture_eng;
    [Export] public Texture2D TextTexture_eng;
    
    public override void _Ready()
    {
        var lang = LocManager.Instance?.Language;
        
        if (lang != "zhs" && lang != "zht")
        {
            if (NodeBG != null && NodeBG.Texture != null)
            {
                NodeBG.Texture = BGTexture_eng;
            }
            if (NodeText != null && NodeText.Texture != null)
            {
                NodeText.Texture = TextTexture_eng;
            }
        }
    }

}
