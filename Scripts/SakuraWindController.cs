using Godot;

namespace STS2_WineFox.Scripts;

/// <summary>
///     樱花粒子风向控制器：监听鼠标位置，轻微影响 GPUParticles2D 的 gravity 方向。
/// </summary>
public partial class SakuraWindController : Node
{
    [Export] public GpuParticles2D? Particles;

    /// <summary>鼠标影响强度（0 = 无影响）</summary>
    [Export] public float MouseInfluence = 0.3f;

    /// <summary>材质更新间隔，避免选人界面每帧写粒子材质。</summary>
    [Export] public float UpdateInterval = 0.05f;

    /// <summary>基础重力方向</summary>
    [Export] public Vector2 BaseGravity = new(0, 20);

    private ParticleProcessMaterial? _material;
    private double _elapsed;
    private Vector2 _lastGravity;

    public override void _Ready()
    {
        _lastGravity = BaseGravity;
        _material = Particles?.ProcessMaterial as ParticleProcessMaterial;
        SetProcess(_material != null && MouseInfluence > 0f);
    }

    public override void _Process(double delta)
    {
        if (_material == null)
            return;

        _elapsed += delta;
        if (_elapsed < UpdateInterval)
            return;

        _elapsed = 0;
        var viewport = GetViewport();
        var screenSize = viewport.GetVisibleRect().Size;
        if (screenSize.X <= 0)
            return;

        var mousePos = viewport.GetMousePosition();
        var center = screenSize / 2;
        var normalizedX = (mousePos.X - center.X) / (screenSize.X / 2);
        normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);

        var gravity = BaseGravity + new Vector2(normalizedX * MouseInfluence * 30f, 0);
        if (_lastGravity.DistanceSquaredTo(gravity) < 0.25f)
            return;

        _lastGravity = gravity;
        _material.Gravity = new Vector3(gravity.X, gravity.Y, 0);
    }
}
