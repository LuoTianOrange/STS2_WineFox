using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2_WineFox.Utils;
using STS2_WineFox.Cards.WineFoxCard;

namespace STS2_WineFox.VFX
{
    /// <summary>
    ///     能量弹的释放特效：以法术图标为贴图，从施法者飞向目标，命中后炸开一圈碎片。
    ///     不依赖场景资产，直接用节点与 Tween 拼出来。
    /// </summary>
    public static class FoxfireBoltVfx
    {
        private const float TravelDuration = 0.28f;
        private const float BurstDuration = 0.32f;
        private const int BurstCount = 7;

        private static readonly Color BoltTint = new(0.55f, 0.95f, 1f);
        private static readonly Color GlowTint = new(0.35f, 0.8f, 1f, 0.5f);

        public static void Play(Creature caster, Creature target)
        {
            var room = NCombatRoom.Instance;
            if (room == null)
                return;

            var casterNode = caster.GetCreatureNode();
            var targetNode = target.GetCreatureNode();
            if (casterNode == null || targetNode == null)
                return;

            var texture = ResourceLoader.Load<Texture2D>(Const.Paths.SpellIconFoxfireBolt);
            if (texture == null)
                return;

            var start = casterNode.GlobalPosition + new Vector2(0f, -90f);
            var end = targetNode.GlobalPosition + new Vector2(0f, -50f);

            var root = new Node2D { GlobalPosition = start, ZIndex = 30 };

            var glow = new Sprite2D
            {
                Texture = texture,
                Modulate = GlowTint,
                Scale = Vector2.One * 1.9f,
            };

            var core = new Sprite2D
            {
                Texture = texture,
                Modulate = BoltTint,
                Scale = Vector2.One * 0.5f,
                Rotation = -0.35f,
            };

            root.AddChild(glow);
            root.AddChild(core);
            room.CombatVfxContainer.AddChildSafely(root);

            TaskHelper.RunSafely(Animate(root, core, glow, start, end, texture));
        }

        private static async Task Animate(
            Node2D root, Sprite2D core, Sprite2D glow,
            Vector2 start, Vector2 end, Texture2D texture)
        {
            var spin = root.CreateTween();
            spin.SetLoops();
            spin.TweenProperty(core, "rotation", 0.35f, 0.12f);
            spin.TweenProperty(core, "rotation", -0.35f, 0.12f);

            var pulse = root.CreateTween();
            pulse.SetLoops();
            pulse.TweenProperty(glow, "scale", Vector2.One * 2.25f, 0.1f);
            pulse.TweenProperty(glow, "scale", Vector2.One * 1.75f, 0.1f);

            var fly = root.CreateTween();
            fly.TweenProperty(root, "global_position", end, TravelDuration)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.In);
            fly.Parallel().TweenProperty(core, "scale", Vector2.One * 0.8f, TravelDuration);
            fly.Parallel().TweenProperty(root, "rotation", 0.6f, TravelDuration);

            await root.ToSignal(fly, Tween.SignalName.Finished);

            if (GodotObject.IsInstanceValid(root))
                root.QueueFreeSafely();

            var room = NCombatRoom.Instance;
            if (room != null)
                SpawnBurst(room, end, texture);
        }

        private static void SpawnBurst(NCombatRoom room, Vector2 position, Texture2D texture)
        {
            var burst = new Node2D { GlobalPosition = position, ZIndex = 30 };
            room.CombatVfxContainer.AddChildSafely(burst);

            for (var i = 0; i < BurstCount; i++)
            {
                var angle = Mathf.Tau * i / BurstCount;
                var shard = new Sprite2D
                {
                    Texture = texture,
                    Scale = Vector2.One * 0.32f,
                    Modulate = BoltTint,
                };

                burst.AddChild(shard);

                var offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 70f;
                var tween = shard.CreateTween();
                tween.TweenProperty(shard, "position", offset, BurstDuration)
                    .SetTrans(Tween.TransitionType.Quad)
                    .SetEase(Tween.EaseType.Out);
                tween.Parallel().TweenProperty(shard, "scale", Vector2.One * 0.06f, BurstDuration);
                tween.Parallel().TweenProperty(shard, "modulate", new Color(BoltTint, 0f), BurstDuration);
            }

            var flash = new Sprite2D
            {
                Texture = texture,
                Scale = Vector2.One * 0.9f,
                Modulate = GlowTint,
            };

            burst.AddChild(flash);

            var flashTween = flash.CreateTween();
            flashTween.TweenProperty(flash, "scale", Vector2.One * 2.6f, BurstDuration)
                .SetTrans(Tween.TransitionType.Quad)
                .SetEase(Tween.EaseType.Out);
            flashTween.Parallel().TweenProperty(flash, "modulate", new Color(GlowTint, 0f), BurstDuration);

            var cleanup = burst.CreateTween();
            cleanup.TweenInterval(BurstDuration + 0.05f);
            cleanup.TweenCallback(Callable.From(() =>
            {
                if (GodotObject.IsInstanceValid(burst))
                    burst.QueueFreeSafely();
            }));
        }
    }
}
