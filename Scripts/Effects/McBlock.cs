using Godot;

namespace STS2_WineFox.Scripts.Effects;

[Tool]
public partial class McBlock : Node2D
{
    private Texture2D? _blockTexture;

    [Export]
    public Texture2D? BlockTexture
    {
        get => _blockTexture;
        set
        {
            _blockTexture = value;
            UpdateMaterial();
        }
    }

    private MeshInstance3D? _mesh;
    private double _time;

    public override void _Ready()
    {
        _mesh = GetNode<MeshInstance3D>("SubViewportContainer/SubViewport/MeshInstance3D");
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if (_mesh == null) return;

        if (_blockTexture != null)
        {
            var mat = new StandardMaterial3D
            {
                AlbedoTexture = _blockTexture,
                TextureFilter = BaseMaterial3D.TextureFilterEnum.Nearest,
                Uv1Triplanar = true
            };
            _mesh.MaterialOverride = mat;
        }
        else
        {
            _mesh.MaterialOverride = null;
        }
    }

    public override void _Process(double delta)
    {
        _time += delta;
        if (_mesh == null) return;

        _mesh.RotateY((float)delta * 2);
        _mesh.RotationDegrees = new Vector3(
            0,
            _mesh.RotationDegrees.Y,
            0
        );
    }
}
