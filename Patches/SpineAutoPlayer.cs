using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace STS2_WineFox.Patches
{
    [GlobalClass]
    public partial class SpineAutoPlayer : Node
    {
        public override void _Ready()
        {
            try
            {
                TryPlayDefaultAnimation();
            }
            catch (Exception ex)
            {
                Main.Logger.Warn($"WineFox character select Spine autoplay failed: {ex}");
            }
        }

        private void TryPlayDefaultAnimation()
        {
            var megaSprite = new MegaSprite(GetParent());
            var animationState = megaSprite.TryGetAnimationState();
            if (animationState == null) return;

            var animationNames = GetAnimationNames(megaSprite);
            if (animationNames.Count == 0) return;

            const string preferredAnimation = "idle";
            var animationToPlay = animationNames[0];

            foreach (var name in animationNames)
            {
                if (!string.Equals(name, preferredAnimation, StringComparison.OrdinalIgnoreCase)) continue;

                animationToPlay = name;
                break;
            }

            animationState.SetAnimation(animationToPlay);
        }

        private static List<string> GetAnimationNames(MegaSprite megaSprite)
        {
            var data = megaSprite.GetSkeleton()?.GetData();
            if (data == null)
                return [];

            var names = data.GetAnimationNames()
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            names.Sort(StringComparer.OrdinalIgnoreCase);
            return names;
        }
    }
}
