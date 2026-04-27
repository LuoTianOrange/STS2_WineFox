using System.Runtime.CompilerServices;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2_WineFox.Character;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    public class WineFoxCreatureLoseHpFlashPatch : IPatchMethod
    {
        public static string PatchId => "winefox_creature_lose_hp_flash";
        public static bool IsCritical => true;
        public static string Description => "Flashes red on damage when WineFox is in party";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(Creature), nameof(Creature.LoseHpInternal))];
        }

        public static void Postfix(Creature __instance, DamageResult __result)
        {
            if (__result.UnblockedDamage <= 0)
                return;
            if (!WineFoxCombatVisualEffects.TryIsWineFoxInParty(__instance))
                return;

            WineFoxCombatVisualEffects.TryPlayHitFlash(__instance);
        }
    }

    public class WineFoxCreatureDeathSmokePlaceholderPatch : IPatchMethod
    {
        public static string PatchId => "winefox_creature_death_smoke_placeholder";
        public static bool IsCritical => true;
        public static string Description => "Invokes death smoke placeholder when WineFox is in party";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(Creature), nameof(Creature.InvokeDiedEvent))];
        }

        public static void Postfix(Creature __instance)
        {
            if (!WineFoxCombatVisualEffects.TryIsWineFoxInParty(__instance))
                return;

            WineFoxCombatVisualEffects.TrySpawnDeathSmokePlaceholder(__instance);
        }
    }

    public class WineFoxCreatureHitTriggerFlashPatch : IPatchMethod
    {
        public static string PatchId => "winefox_creature_hit_trigger_flash";
        public static bool IsCritical => true;
        public static string Description => "Flashes red when Hit animation trigger is played";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))];
        }

        public static void Postfix(NCreature __instance, string trigger)
        {
            if (!string.Equals(trigger, "Hit", StringComparison.Ordinal))
                return;
            if (__instance.Entity == null || !WineFoxCombatVisualEffects.TryIsWineFoxInParty(__instance.Entity))
                return;

            WineFoxCombatVisualEffects.TryPlayHitFlash(__instance.Entity);
        }
    }

    internal static class WineFoxCombatVisualEffects
    {
        private const float HitFlashHoldDuration = 0.1f;
        private const float HitFlashFadeDuration = 0.28f;
        private static readonly ConditionalWeakTable<NCreature, FlashState> FlashStates = new();
        private static readonly Color HitFlashColor = new(1f, 0.05f, 0.05f);

        public static bool TryIsWineFoxInParty(Creature creature)
        {
            var combatState = creature.CombatState;
            if (combatState == null)
                return false;

            foreach (var player in combatState.Players)
                if (player.Character is WineFox
                    || player.Character.Id.Entry.Contains("winefox", StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }

        public static void TryPlayHitFlash(Creature creature)
        {
            var creatureNode = creature.GetCreatureNode();
            if (creatureNode?.Visuals == null || !GodotObject.IsInstanceValid(creatureNode.Visuals))
                return;

            var state = FlashStates.GetValue(creatureNode, static _ => new());
            state.Tween?.Kill();
            state.Tween = null;

            var visuals = creatureNode.Visuals;
            if (!state.IsFlashing)
                state.BaseModulate = visuals.Modulate;
            state.IsFlashing = true;
            visuals.Modulate = new(HitFlashColor.R, HitFlashColor.G, HitFlashColor.B, state.BaseModulate.A);

            var tween = visuals.CreateTween();
            state.Tween = tween;
            tween.TweenInterval(HitFlashHoldDuration);
            tween.TweenProperty(visuals, "modulate", state.BaseModulate, HitFlashFadeDuration)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.Out);
            tween.Finished += () =>
            {
                if (!GodotObject.IsInstanceValid(visuals))
                    return;
                visuals.Modulate = state.BaseModulate;
                state.Tween = null;
                state.IsFlashing = false;
            };
        }

        public static void TrySpawnDeathSmokePlaceholder(Creature creature)
        {
            Main.Logger.Debug($"[WineFoxVfx] Death smoke placeholder called for {creature.ModelId.Entry}.");
        }

        private sealed class FlashState
        {
            public Tween? Tween { get; set; }
            public Color BaseModulate { get; set; } = Colors.White;
            public bool IsFlashing { get; set; }
        }
    }
}
