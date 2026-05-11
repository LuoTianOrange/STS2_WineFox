using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace STS2_WineFox.Patches;

[GlobalClass]
public partial class SpineAutoPlayer : Node
{
	public override void _Ready()
	{
		var megaSprite = new MegaSprite(GetParent());
		var skeleton = megaSprite.GetSkeleton();
		if (skeleton == null) return;

		var animations = skeleton.GetData().GetAnimations();
		if (animations.Count == 0) return;

		const string preferredAnimation = "idle";
		var animationToPlay = new MegaAnimation(animations[0]).GetName();

		foreach (var animation in animations)
		{
			var name = new MegaAnimation(animation).GetName();
			if (name != preferredAnimation) continue;

			animationToPlay = name;
			break;
		}

		megaSprite.GetAnimationState().SetAnimation(animationToPlay);
	}
}
