using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2_WineFox.Settings;

namespace STS2_WineFox.Character
{
    internal static class WineFoxStandee
    {
        private const string DiePortraitNodeName = "WineFoxDiePortrait";
        private const string VariantMeta = "winefox_standee_variant";
        private static readonly Vector2 DiePortraitPosition = new(4f, -110f);
        private const float DiePortraitScale = 0.3f;
        private static readonly List<NCreatureVisuals> Live = [];

        public static bool IsWineFoxCharacter(Creature? creature)
        {
            return creature?.Player?.Character is WineFox;
        }

        public static void ApplyVariantIfNeeded(NCreatureVisuals visuals)
        {
            try
            {
                Register(visuals);
                if (visuals.GetParent() is NCreature creatureNode)
                    ApplyVariantFor(visuals, creatureNode);
            }
            catch (Exception ex)
            {
                GD.PushWarning($"[WineFox] standee variant apply failed: {ex}");
            }
        }

        public static void ApplyVariantForNode(NCreature creatureNode)
        {
            try
            {
                if (creatureNode.Visuals is not { } visuals)
                    return;

                Register(visuals);
                ApplyVariantFor(visuals, creatureNode);
            }
            catch (Exception ex)
            {
                GD.PushWarning($"[WineFox] standee variant apply failed: {ex}");
            }
        }

        public static void RefreshExisting()
        {
            NCreatureVisuals[] snapshot;
            lock (Live)
            {
                Live.RemoveAll(static visuals => !GodotObject.IsInstanceValid(visuals));
                snapshot = [.. Live];
            }

            foreach (var visuals in snapshot)
                ApplyVariantIfNeeded(visuals);
        }

        private static void Register(NCreatureVisuals visuals)
        {
            lock (Live)
            {
                if (!Live.Contains(visuals))
                    Live.Add(visuals);
            }
        }

        private static void ApplyVariantFor(NCreatureVisuals visuals, NCreature creatureNode)
        {
            if (!IsWineFoxCharacter(creatureNode.Entity))
                return;

            if (visuals.SpineBody is not { } spineBody)
                return;

            var wanted = WineFoxStandeeSync.UsesOldStandee(creatureNode.Entity?.Player)
                ? WineFoxStandeeVariants.Old
                : WineFoxStandeeVariants.New;

            var current = visuals.HasMeta(VariantMeta)
                ? visuals.GetMeta(VariantMeta).AsString()
                : WineFoxStandeeVariants.New;

            if (current == wanted)
                return;

            if (ResourceLoader.Load(WineFoxStandeeVariants.SkeletonPath(wanted)) is not { } skeletonData)
                return;

            var previousHeight = spineBody.GetSkeleton()?.GetBounds().Size.Y ?? 0f;

            spineBody.SetSkeletonDataRes(new MegaSkeletonDataResource(skeletonData));

            var currentHeight = spineBody.GetSkeleton()?.GetBounds().Size.Y ?? 0f;
            if (previousHeight > 1f && currentHeight > 1f && visuals.GetCurrentBody() is { } body)
                body.Scale *= Mathf.Clamp(previousHeight / currentHeight, 0.25f, 6f);

            visuals.SetMeta(VariantMeta, wanted);
        }

        public static void ShowDiePortrait(NCreature creatureNode)
        {
            if (!WineFoxStandeeSync.UsesOldStandee(creatureNode.Entity?.Player) ||
                !IsWineFoxCharacter(creatureNode.Entity))
                return;

            var visuals = creatureNode.Visuals;
            if (visuals == null)
                return;

            HideDiePortrait(creatureNode);

            var spineBody = visuals.GetCurrentBody();
            if (spineBody != null)
                spineBody.Visible = false;

            var texture = ResourceLoader.Load<Texture2D>(Const.Paths.StandeeDiePortrait);
            if (texture == null)
                return;

            var portrait = new Sprite2D
            {
                Name = DiePortraitNodeName,
                Texture = texture,
                Position = DiePortraitPosition,
                Scale = Vector2.One * DiePortraitScale,
            };

            visuals.AddChild(portrait);
        }

        public static void HideDiePortrait(NCreature creatureNode)
        {
            var visuals = creatureNode.Visuals;
            if (visuals == null)
                return;

            var existing = visuals.GetNodeOrNull<Sprite2D>(DiePortraitNodeName);
            if (existing != null)
            {
                visuals.RemoveChild(existing);
                existing.QueueFree();
            }

            var spineBody = visuals.GetCurrentBody();
            if (spineBody != null)
                spineBody.Visible = true;
        }
    }
}
