using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2_WineFox.Nodes;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Scaffolding.Godot.NodeAttachments;

namespace STS2_WineFox.Patches
{
    /// <summary>
    ///     战斗界面激活时，把法杖槽位预览条绑定到本地玩家的槽位容器。
    ///     <para>
    ///         槽位数据来自 <c>MagicWineFoxSpellSlotPower</c>，不是游戏原生法球，
    ///         因此预览条是自建节点，而非 Noita 那样复用 <c>NOrbManager</c>。
    ///     </para>
    /// </summary>
    internal sealed class NCombatUiActivateSpellSlotBarPatch : IPatchMethod
    {
        public static string PatchId => "winefox_spell_slot_bar_combat_ui_activate";
        public static string Description => "Bind WineFox wand slot preview bar alongside NCombatUi.Activate";
        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCombatUi), nameof(NCombatUi.Activate), [typeof(CombatState)])];
        }

        public static void Postfix(NCombatUi __instance, CombatState state)
        {
            if (!SpellSlotBarPatches.TryGetBar(__instance, out var bar))
                return;

            var me = LocalContext.GetMe(state);
            bar.Bind(me);
        }
    }

    internal sealed class NCombatUiAnimOutSpellSlotBarPatch : IPatchMethod
    {
        public static string PatchId => "winefox_spell_slot_bar_combat_ui_anim_out";
        public static string Description => "Hide WineFox wand slot preview bar alongside NCombatUi.AnimOut";
        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCombatUi), nameof(NCombatUi.AnimOut))];
        }

        public static void Postfix(NCombatUi __instance)
        {
            if (SpellSlotBarPatches.TryGetBar(__instance, out var bar))
                bar.Unbind();
        }
    }

    internal sealed class NCombatUiDeactivateSpellSlotBarPatch : IPatchMethod
    {
        public static string PatchId => "winefox_spell_slot_bar_combat_ui_deactivate";
        public static string Description => "Hide WineFox wand slot preview bar alongside NCombatUi.Deactivate";
        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCombatUi), nameof(NCombatUi.Deactivate))];
        }

        public static void Postfix(NCombatUi __instance)
        {
            if (SpellSlotBarPatches.TryGetBar(__instance, out var bar))
                bar.Unbind();
        }
    }

    internal static class SpellSlotBarPatches
    {
        public static bool TryGetBar(NCombatUi combatUi, out NSpellSlotBar bar)
        {
            ModNodeAttachmentRegistry.EnsureReadyAttachments(combatUi);
            return ModNodeAttachmentRegistry.For(Const.ModId)
                .TryGetAttached<NCombatUi, NSpellSlotBar>(
                    combatUi,
                    NSpellSlotBar.AttachmentId,
                    out bar);
        }
    }
}
