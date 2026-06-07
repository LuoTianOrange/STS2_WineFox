using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using STS2_WineFox.Utils;

namespace STS2_WineFox.Scripts.Effects;

public partial class VfxBasicMine : Node2D
{
    [Export] public float GroundY { get; set; } = 173f;
    [Export] public float Phase1Duration { get; set; } = 0.3f;
    [Export] public float Phase3Duration { get; set; } = 0.3f;
    [Export] public float HorizontalScatter { get; set; } = 120f;

    private readonly List<Node2D> _blocks = new();
    private int _completedCount;

    public override void _Ready()
    {
        foreach (var child in GetChildren())
        {
            if (child is Node2D block)
            {
                _blocks.Add(block);
            }
        }

        foreach (var block in _blocks)
        {
            _ = RunBlockAnimation(block);
        }
    }

    private async Task RunBlockAnimation(Node2D block)
    {
        var random = new Random(block.GetInstanceId().GetHashCode());
        var startPos = block.Position;

        // Phase 1: 抛物线落下
        float groundY = (float)random.NextDouble() * 25f + GroundY;
        var landX = startPos.X + (float)(random.NextDouble() * HorizontalScatter * 2 - HorizontalScatter);
        var landPos = new Vector2(landX, groundY);

        var tween1 = CreateTween();
        tween1.SetTrans(Tween.TransitionType.Quad);
        tween1.SetEase(Tween.EaseType.In);
        tween1.TweenProperty(block, "position", landPos, Phase1Duration);
        await ToSignal(tween1, Tween.SignalName.Finished);

        // Phase 2: 地面微微浮动 (1s ~ 2s，每个方块不同)
        var floatDuration = 0.5f + 0.7f * (float)random.NextDouble();
        await VFXUtil.Wait(floatDuration);

        // Phase 3: 吸入中心 (0,0)，先慢后快，同时缩小
        var tween3 = CreateTween();
        tween3.SetTrans(Tween.TransitionType.Cubic);
        tween3.SetEase(Tween.EaseType.In);
        tween3.TweenProperty(block, "position", Vector2.Zero, Phase3Duration);
        // tween3.Parallel().TweenProperty(block, "scale", new Vector2(0.4f, 0.4f), Phase3Duration);
        await ToSignal(tween3, Tween.SignalName.Finished);
        
        VFXUtil.PlaySFXSimple(Const.Audio.PickUp, .6f);
        block.QueueFree();

        _completedCount++;
        if (_completedCount >= _blocks.Count)
        {
            QueueFree();
        }
    }
}
