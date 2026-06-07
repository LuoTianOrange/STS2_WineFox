using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;
using STS2RitsuLib.Audio;


namespace STS2_WineFox.Utils
{
    /// <summary>
    ///     VFX 视觉特效常用Util 从万象辉星迁移【
    /// </summary>
    public static class VFXUtil
    {
        public static AudioPlayResult PlaySFXSimple(string path)
        {
            return GameAudioService.Shared.PlayOneShot(AudioSource.Event(path));
        }
        
        public static AudioPlayResult PlaySFXSimple(string path, float volume)
        {
            return GameAudioService.Shared.PlayOneShot(AudioSource.Event(path), new ()
            {
                Volume = volume,
            });
        }

        public static Vector2 RandVec2(float beta)
        {
            return new Vector2((float)GD.RandRange(-1f, 1f) * beta, (float)GD.RandRange(-1f, 1f) * beta);
        }

        public static void FitVFX(
            this Node2D node,
            Vector2 nodeStartPos,
            Vector2 nodeEndPos,
            Vector2 sceneStartPos,
            Vector2 sceneEndPos
        )
        {
            Vector2 originalVec = nodeStartPos - nodeEndPos;
            Vector2 targetVec = sceneStartPos - sceneEndPos;

            // 计算旋转角度（弧度）
            float angle = targetVec.Angle() - originalVec.Angle();
            // 计算均匀缩放因子
            float scale = targetVec.Length() / originalVec.Length();

            node.Rotation = angle;
            node.Scale = Vector2.One * scale;
        }

        public static Node2D? PlaySimple(string scenePath, Vector2 position, float lifetime = 2f)
        {
            if (!TestMode.IsOn && NCombatRoom.Instance != null)
            {
                Node2D node2D = GenVFXNode(scenePath);
                NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(node2D);
                node2D.GlobalPosition = position;

                if (lifetime > 0f)
                {
                    SceneTreeTimer timer = node2D.GetTree().CreateTimer(lifetime);
                    timer.Timeout += () =>
                    {
                        if (GodotObject.IsInstanceValid(node2D))
                        {
                            node2D.QueueFreeSafely();
                        }
                    };
                }
                return node2D;
            }

            return null;
        }

        public static Node2D? PlaySimpleBack(string scenePath, Vector2 position, float lifetime = 2f)
        {
            if (!TestMode.IsOn && NCombatRoom.Instance != null)
            {
                Node2D node2D = GenVFXNode(scenePath);
                NCombatRoom.Instance.BackCombatVfxContainer.AddChildSafely(node2D);
                node2D.GlobalPosition = position;

                SceneTreeTimer timer = node2D.GetTree().CreateTimer(lifetime);
                timer.Timeout += () =>
                {
                    if (GodotObject.IsInstanceValid(node2D))
                    {
                        node2D.QueueFreeSafely();
                    }
                };
                return node2D;
            }

            return null;
        }

        public static async void ShakeAfter(float time, ShakeStrength strength, ShakeDuration duration,
            float degAngle = -1f)
        {
            await VFXUtil.Wait(time);
            NGame.Instance?.ScreenShake(strength, duration, degAngle);
        }

        //这里重写Cmd.Wait函数，是因为皮皮极速等Mod Patch了Cmd.Wait, 为了防止影响简单抄写了一遍
        public static Task Wait(float seconds, bool ignoreCombatEnd = false)
        {
            return VFXUtil.Wait(seconds, new CancellationToken(), ignoreCombatEnd);
        }

        public static async Task Wait(float seconds, CancellationToken cancelToken, bool ignoreCombatEnd = false)
        {
            if (NonInteractiveMode.IsActive || (double)seconds <= 0.0 || NGame.Instance != null &&
                (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant ||
                 !ignoreCombatEnd && CombatManager.Instance.IsEnding))
                return;
            await VFXUtil.WaitInternal(((SceneTree)Engine.GetMainLoop()).CreateTimer((double)seconds), cancelToken);
        }

        public static Task WaitInternal(SceneTreeTimer timer, CancellationToken cancellationToken)
        {
            TaskCompletionSource tcs = new TaskCompletionSource();
            timer.Timeout += () => { tcs.TrySetResult(); };
            if (cancellationToken.CanBeCanceled)
                cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            return tcs.Task;
        }

        public static async Task CustomScaledWait(
            float fastSeconds,
            float standardSeconds,
            bool ignoreCombatEnd = false,
            CancellationToken cancellationToken = default)
        {
            if (NonInteractiveMode.IsActive || SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant ||
                !ignoreCombatEnd && CombatManager.Instance.IsEnding)
                return;
            switch (SaveManager.Instance.PrefsSave.FastMode)
            {
                case FastModeType.Normal:
                    await VFXUtil.Wait(standardSeconds, cancellationToken, ignoreCombatEnd);
                    break;
                case FastModeType.Fast:
                    await VFXUtil.Wait(fastSeconds, cancellationToken, ignoreCombatEnd);
                    break;
                case FastModeType.Instant:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static T? PlaySimple<T>(string scenePath, Vector2 position) where T : Node2D
        {
            if (!TestMode.IsOn && NCombatRoom.Instance != null)
            {
                T node2D = GenVFXNode<T>(scenePath);
                NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(node2D);
                node2D.GlobalPosition = position;
                return node2D;
            }

            return null;
        }

        public static Node2D GenVFXNode(string scenePath)
        {
            if (VFXCache.ModSceneCache.TryGetValue(scenePath, out var modScene))
            {
                return modScene.Instantiate<Node2D>();
            }

            return PreloadManager.Cache.GetScene(scenePath).Instantiate<Node2D>();
        }

        public static T GenVFXNode<T>(string scenePath) where T : Node2D
        {
            if (VFXCache.ModSceneCache.TryGetValue(scenePath, out var modScene))
            {
                return modScene.Instantiate<T>();
            }

            return PreloadManager.Cache.GetScene(scenePath).Instantiate<T>();
        }

        public static void ReplayAllParticles(Node2D node)
        {
            if (node is GpuParticles2D particles)
            {
                particles.Restart();
            }

            foreach (Node child in node.GetChildren())
            {
                if (child is Node2D childNode)
                {
                    ReplayAllParticles(childNode);
                }
            }
        }
    }

    public static class RandomHelper
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// 从一个List中随机取出一个值
        /// </summary>
        /// <typeparam name="T">List中元素的类型</typeparam>
        /// <param name="list">源List（不能为空）</param>
        /// <returns>随机选择的一个元素</returns>
        public static T VRand<T>(this List<T> list)
        {
            // 不考虑list为空的情况，直接取随机索引
            int index = _random.Next(list.Count);
            return list[index];
        }
    }
}
