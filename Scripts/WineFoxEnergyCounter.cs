using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace STS2_WineFox.Scripts
{
	public partial class WineFoxEnergyCounter : NEnergyCounter
	{
		public override void _Ready()
		{
			base._Ready();

			// WineFox default burst tint
			var burst = new Color(0.964706f, 0.611765f, 0.768627f, 0.6f);
			if (GetNodeOrNull<CpuParticles2D>("%BurstBack") is { } back)
				back.Color = burst;
			if (GetNodeOrNull<CpuParticles2D>("%BurstFront") is { } front)
				front.Color = burst;
		}
	}
}
