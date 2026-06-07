using Godot;
using System.Collections.Generic;

namespace STS2_WineFox.Scripts.Effects;

/// <summary>
///     自动播放挂载节点及其子树下的所有 <see cref="GpuParticles2D"/>，
///     并在最长生命周期结束后自动销毁（QueueFree）自身。
/// </summary>
public partial class NParticlesAutoPlayer : Node2D
{
    public override void _Ready()
    {
        var particles = new List<GpuParticles2D>();
        CollectParticles(this, particles);

        double maxDuration = 0.0;

        foreach (var p in particles)
        {
            p.Emitting = true;
            p.Restart();

            if (p.Lifetime > maxDuration)
                maxDuration = p.Lifetime;
        }

        if (maxDuration > 0.0)
        {
            GetTree().CreateTimer(maxDuration).Timeout += () => QueueFree();
        }
        else
        {
            QueueFree();
        }
    }

    private void CollectParticles(Node node, List<GpuParticles2D> list)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is GpuParticles2D gpu)
                list.Add(gpu);

            CollectParticles(child, list);
        }
    }
}
