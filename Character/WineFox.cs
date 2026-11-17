using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Characters;
using STS2_WineFox.Content.Descriptors;
using STS2_WineFox.Epoch;
using STS2_WineFox.Settings;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;

namespace STS2_WineFox.Character
{
    [RegisterCharacter]
    [RequireEpoch(typeof(WineFoxCharacterEpoch))]
    [UnlockEpochAfterRunAs(typeof(WineFoxCardEpoch))]
    public class WineFox : ModCharacterTemplate<WineFoxCardPool, WineFoxRelicPool, WineFoxPotionPool>
    {
        public static readonly Color Color = new("ffaf50");

        public override Color NameColor => Color;
        public override Color MapDrawingColor => Colors.Orange;
        public override int StartingHp => 80;
        public override int StartingGold => 99;
        public override CharacterGender Gender => CharacterGender.Neutral;
        public override CharacterAssetProfile AssetProfile => WineFoxCharacterAssets.Profile;
        public override float AttackAnimDelay => 0.15f;
        public override float CastAnimDelay => 0.25f;

        protected override Type UnlocksAfterRunAsType => typeof(Ironclad);

        public override CreatureAnimator GenerateAnimator(MegaSprite controller, Creature creature)
        {
            if (!WineFoxStandeeSync.UsesOldStandee(creature.Player))
                return base.GenerateAnimator(controller, creature);

            var idle = new AnimState(ResolveAnimation(controller, AnimState.idleAnim, "idle", "Idle"), isLooping: true);
            var animator = new CreatureAnimator(idle, controller);

            animator.AddAnyState("Idle", idle);
            animator.AddAnyState("Relaxed", idle);
            animator.AddAnyState("Dead", idle);

            foreach (var (state, trigger) in AnimationStates)
            {
                if (!controller.HasAnimation(state.Id))
                    continue;

                state.AddNextState(idle);
                animator.AddAnyState(trigger, state);
            }

            return animator;
        }

        private static string ResolveAnimation(MegaSprite controller, params string[] candidates)
        {
            foreach (var candidate in candidates)
            {
                if (controller.HasAnimation(candidate))
                    return candidate;
            }

            var names = controller.GetSkeleton()?.GetData()?.GetAnimationNames();
            if (names == null)
                return candidates[0];

            foreach (var name in names)
            {
                if (name.IndexOf("idle", StringComparison.OrdinalIgnoreCase) >= 0)
                    return name;
            }

            return names.Count > 0 ? names[0] : candidates[0];
        }

        public override List<string> GetArchitectAttackVfx()
        {
            return
            [
                "vfx/vfx_attack_blunt",
                "vfx/vfx_heavy_blunt",
                "vfx/vfx_attack_slash",
            ];
        }
    }
}
