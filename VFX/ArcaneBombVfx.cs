using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2_WineFox.Utils;

namespace STS2_WineFox.VFX
{
    /// <summary>
    ///     奥术炸弹的释放特效：紫色法术图标从施法者抛向每个目标，落地后炸成一圈向外扩散的碎片与冲击波。
    ///     纯节点与 Tween 实现，不依赖场景资产。
    /// </summary>
    public static class ArcaneBombVfx
    {
        private const float TossUp = 0.14f;
        private const float TossDown = 0.16f;
        private const float BurstDuration = 0.36f;
        private const int ShardCount = 10;

        private static readonly Color CoreTint = new(0.8f, 0.55f, 1f);
        private static readonly Color GlowTint = new(0.58f, 0.28f, 1f, 0.55f);
        private static readonly Color RingTint = new(0.72f, 0.45f, 1f, 0.35f);

        public static void Play(Creature caster, IReadOnlyList<Creature> targets)
        {
            var room = NCombatRoom.Instance;
            if (room == null || targets.Count == 0)
                return;

            var casterNode = caster.GetCreatureNode();
            if (casterNode == null)
                return;

            var texture = ResourceLoader.Load<Texture2D>(Const.Paths.SpellIconArcaneBomb);
            if (texture == null)
                return;

            var start = casterNode.GlobalPosition + new Vector2(0f, -95f);

            foreach (var target in targets)
            {
                var targetNode = target.GetCreatureNode();
                if (targetNode == null)
                    continue;

                var end = targetNode.GlobalPosition + new Vector2(0f, -55f);
                ThrowBomb(room, texture, start, end);
            }
        }

        private static void ThrowBomb(NCombatRoom room, Texture2D texture, Vector2 start, Vector2 end)
        {
            var bomb = new Node2D { GlobalPosition = start, ZIndex = 30 };

            var glow = new Sprite2D
            {
                Texture = texture,
                Modulate = GlowTint,
                Scale = Vector2.One * 1.3f,
            };

            var core = new Sprite2D
            {
                Texture = texture,
                Modulate = CoreTint,
                Scale = Vector2.One * 0.42f,
            };

            bomb.AddChild(glow);
            bomb.AddChild(core);
            room.CombatVfxContainer.AddChildSafely(bomb);

            var apex = new Vector2((start.X + end.X) * 0.5f, Mathf.Min(start.Y, end.Y) - 70f);

            var toss = bomb.CreateTween();
            toss.TweenProperty(bomb, "global_position", apex, TossUp)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.Out);
            toss.TweenProperty(bomb, "global_position", end, TossDown)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.In);
            toss.Parallel().TweenProperty(core, "rotation", 3.4f, TossUp + TossDown);
            toss.TweenCallback(Callable.From(() =>
            {
                if (GodotObject.IsInstanceValid(bomb))
                    bomb.QueueFreeSafely();

                if (GodotObject.IsInstanceValid(room) && room.CombatVfxContainer != null)
                    SpawnExplosion(room, texture, end);
            }));
        }

        private static void SpawnExplosion(NCombatRoom room, Texture2D texture, Vector2 position)
        {
            var burst = new Node2D { GlobalPosition = position, ZIndex = 30 };
            room.CombatVfxContainer.AddChildSafely(burst);

            var flash = new Sprite2D
            {
                Texture = texture,
                Modulate = CoreTint,
                Scale = Vector2.One * 0.7f,
            };

            burst.AddChild(flash);

            var flashTween = flash.CreateTween();
            flashTween.TweenProperty(flash, "scale", Vector2.One * 3.2f, BurstDuration)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.Out);
            flashTween.Parallel().TweenProperty(flash, "modulate", new Color(CoreTint, 0f), BurstDuration);

            for (var ringIndex = 0; ringIndex < 2; ringIndex++)
            {
                var ring = new Sprite2D
                {
                    Texture = texture,
                    Modulate = RingTint,
                    Scale = Vector2.One * 0.6f,
                };

                burst.AddChild(ring);

                var ringTween = ring.CreateTween();
                ringTween.TweenInterval(ringIndex * 0.07f);
                ringTween.TweenProperty(ring, "scale", Vector2.One * 4.6f, BurstDuration)
                    .SetTrans(Tween.TransitionType.Quad)
                    .SetEase(Tween.EaseType.Out);
                ringTween.Parallel().TweenProperty(ring, "modulate", new Color(RingTint, 0f), BurstDuration);
            }

            for (var i = 0; i < ShardCount; i++)
            {
                var angle = Mathf.Tau * i / ShardCount;
                var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                var shard = new Sprite2D
                {
                    Texture = texture,
                    Modulate = CoreTint,
                    Scale = Vector2.One * 0.28f,
                };

                burst.AddChild(shard);

                var tween = shard.CreateTween();
                tween.TweenProperty(shard, "position", direction * 110f, BurstDuration)
                    .SetTrans(Tween.TransitionType.Quad)
                    .SetEase(Tween.EaseType.Out);
                tween.Parallel().TweenProperty(shard, "scale", Vector2.One * 0.05f, BurstDuration);
                tween.Parallel().TweenProperty(shard, "rotation", direction.X * 2.4f, BurstDuration);
                tween.Parallel().TweenProperty(shard, "modulate", new Color(CoreTint, 0f), BurstDuration);
            }

            var cleanup = burst.CreateTween();
            cleanup.TweenInterval(BurstDuration + 0.2f);
            cleanup.TweenCallback(Callable.From(() =>
            {
                if (GodotObject.IsInstanceValid(burst))
                    burst.QueueFreeSafely();
            }));
        }
    }
}
