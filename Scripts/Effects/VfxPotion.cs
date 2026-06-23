using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2_WineFox.Utils;

namespace STS2_WineFox.Scripts.Effects;

/// <summary>
///    调用VfxPotion.Create()来生成药水效果
/// </summary>
public partial class VfxPotion : Node2D
{
    [Export] private GpuParticles2D _particles;

    private ParticleProcessMaterial _mat;

    public Color color;
    
    public static VfxPotion Create(Color color, Vector2 position)
    {
        var node = VFXUtil.GenVFXNode<VfxPotion>(Const.Paths.PotionVfx);
        node.color = color;
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(node);
        node.GlobalPosition = position;
        return node;
    }

    public void SyncColor()
    {
        _mat.Color = color;
    }
    public override void _Ready()
    {
        if (_particles != null)
        {
            if (_particles.ProcessMaterial is ParticleProcessMaterial mat)
            {
                _mat = mat.Duplicate() as ParticleProcessMaterial;
                SyncColor();
                _particles.ProcessMaterial = _mat;
            }
            // _particles.SelfModulate = color;
            _particles.Emitting = true;
            _particles.Restart();
            if (_particles.Lifetime > 0)
                GetTree().CreateTimer(_particles.Lifetime).Timeout += () => QueueFree();
            else
            {
                QueueFree();
            }
        }
    }
}
