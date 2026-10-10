using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using STS2_WineFox.Utils;

namespace STS2_WineFox.VFX
{
    /// <summary>
    ///     汇聚之光的绿色光束。
    ///     <para>
    ///         复用原版超能光束的节点树（准备粒子 → 光束 → 收尾爆发），
    ///         在实例化后把 <c>Line2D</c> 与粒子统一染成绿色，并替换短促的绿色音效。
    ///         这样不需要新的场景资产。
    ///     </para>
    /// </summary>
    public static class GatheringLightVfx
    {
        /// <summary>汇聚之光的主色（青绿）。</summary>
        private static readonly Color BeamGreen = new(0.25f, 1f, 0.45f);

        private const string BeamSfx = "event:/sfx/characters/defect/defect_hyperbeam";

        /// <summary>播放一次从施法者射向目标的绿色光束。</summary>
        public static void Play(Creature owner, Creature target)
        {
            var room = NCombatRoom.Instance;
            if (room == null)
                return;

            var beam = NHyperbeamVfx.Create(owner, target);
            if (beam == null)
                return;

            ApplyGreen(beam);
            room.CombatVfxContainer.AddChildSafely(beam);
            TaskHelper.RunSafely(PlaySequence(beam));
        }

        /// <summary>递归把光束节点树里的线与粒子染绿。</summary>
        private static void ApplyGreen(Node node)
        {
            if (node is Line2D line)
            {
                line.Modulate = BeamGreen;
            }
            else if (node is GpuParticles2D particles)
            {
                particles.Modulate = BeamGreen;
                particles.SelfModulate = BeamGreen;
            }

            foreach (var child in node.GetChildren())
                ApplyGreen(child);
        }

        private static async Task PlaySequence(NHyperbeamVfx beam)
        {
            VFXUtil.PlaySFXSimple(BeamSfx);

            await VFXUtil.Wait(NHyperbeamVfx.hyperbeamAnticipationDuration);
            if (!GodotObject.IsInstanceValid(beam))
                return;

            NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Normal);

            await VFXUtil.Wait(NHyperbeamVfx.hyperbeamLaserDuration);
            if (!GodotObject.IsInstanceValid(beam))
                return;

            NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Short);
            beam.QueueFreeSafely();
        }
    }
}
