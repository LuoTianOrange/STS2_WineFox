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

    /// <summary>风向平滑插值速度</summary>
    [Export] public float SmoothSpeed = 2.0f;

    /// <summary>基础重力方向</summary>
    [Export] public Vector2 BaseGravity = new(0, 20);

    private Vector2 _currentGravity;
    private Vector2 _targetGravity;

    public override void _Ready()
    {
        _currentGravity = BaseGravity;
        _targetGravity = BaseGravity;
    }

    public override void _Process(double delta)
    {
        if (Particles == null)
            return;

        // 计算鼠标相对于屏幕中心的影响
        var viewport = GetViewport();
        var mousePos = viewport.GetMousePosition();
        var screenSize = viewport.GetVisibleRect().Size;
        var center = screenSize / 2;

        // 鼠标在屏幕中心的水平偏移 (-1 ~ 1)
        var normalizedX = (mousePos.X - center.X) / (screenSize.X / 2);
        normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);

        // 目标重力 = 基础重力 + 鼠标影响
        _targetGravity = BaseGravity + new Vector2(normalizedX * MouseInfluence * 30f, 0);

        // 平滑插值
        _currentGravity = _currentGravity.Lerp(_targetGravity, (float)delta * SmoothSpeed);

        // 更新粒子系统的重力
        if (Particles.ProcessMaterial is ParticleProcessMaterial material)
        {
            material.Gravity = new Vector3(_currentGravity.X, _currentGravity.Y, 0);
            // Main.Logger.Info("gravity:" + _currentGravity.X);
        }
    }
}
