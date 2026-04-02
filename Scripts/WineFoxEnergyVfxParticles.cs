using System.Reflection;
using Godot;
using Godot.Collections;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace STS2_WineFox.Scripts
{
    /// <summary>
    ///     Mod 内 NParticlesContainer 子类：场景不序列化基类私有 <c>_particles</c>（跨程序集导出会失败），
    ///     在 <see cref="_Ready" /> 中从子节点收集 <see cref="GpuParticles2D" /> 并写入基类字段。
    /// </summary>
    public partial class WineFoxEnergyVfxParticles : NParticlesContainer
    {
        private static readonly FieldInfo ParticlesField =
            typeof(NParticlesContainer).GetField("_particles", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("NParticlesContainer._particles not found");

        public override void _Ready()
        {
            if (ParticlesField.GetValue(this) is not Array<GpuParticles2D> { Count: > 0 })
            {
                var arr = new Array<GpuParticles2D>();
                foreach (var child in GetChildren())
                    if (child is GpuParticles2D gpu)
                        arr.Add(gpu);
                ParticlesField.SetValue(this, arr);
            }

            base._Ready();
        }
    }
}
