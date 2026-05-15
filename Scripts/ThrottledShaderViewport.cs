using Godot;

namespace STS2_WineFox.Scripts;

/// <summary>
///     Renders a costly animated overlay into a low-resolution SubViewport at a capped rate.
/// </summary>
public partial class ThrottledShaderViewport : Node
{
    [Export] public SubViewport? LightViewport;
    [Export] public Control? ShaderRect;
    [Export] public TextureRect? Output;
    [Export] public Vector2I RenderSize = new(640, 360);
    [Export] public float UpdatesPerSecond = 15f;

    private double _elapsed;
    private double UpdateInterval => UpdatesPerSecond <= 0f ? double.PositiveInfinity : 1.0 / UpdatesPerSecond;

    public override void _Ready()
    {
        if (LightViewport == null || Output == null)
        {
            SetProcess(false);
            return;
        }

        LightViewport.Size = RenderSize;
        LightViewport.TransparentBg = true;
        LightViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;

        if (ShaderRect != null)
            ShaderRect.Size = RenderSize;

        Output.Texture = LightViewport.GetTexture();
        Output.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        Output.StretchMode = TextureRect.StretchModeEnum.Scale;
        Output.MouseFilter = Control.MouseFilterEnum.Ignore;
    }

    public override void _Process(double delta)
    {
        if (LightViewport == null)
            return;

        _elapsed += delta;
        if (_elapsed < UpdateInterval)
            return;

        _elapsed = 0;
        LightViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
    }
}
