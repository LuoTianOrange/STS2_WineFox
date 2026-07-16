using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2_WineFox.Character;
using STS2_WineFox.Combat;
using STS2_WineFox.Powers;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Scaffolding.Godot.NodeAttachments;

namespace STS2_WineFox.Patches
{
    internal sealed class NCombatUiActivateMaterialInventoryHudPatch : IPatchMethod
    {
        public static string PatchId => "winefox_material_inventory_hud_combat_ui_activate";
        public static string Description => "Bind WineFox material inventory HUD alongside NCombatUi.Activate";
        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCombatUi), nameof(NCombatUi.Activate), [typeof(CombatState)])];
        }

        public static void Postfix(NCombatUi __instance, CombatState state)
        {
            if (!MaterialInventoryHudPatches.TryGetHud(__instance, out var hud)) return;

            hud.BindPlayers(state.Players);
        }
    }

    internal sealed class NCombatUiAnimOutMaterialInventoryHudPatch : IPatchMethod
    {
        public static string PatchId => "winefox_material_inventory_hud_combat_ui_anim_out";
        public static string Description => "Hide WineFox material inventory HUD alongside NCombatUi.AnimOut";
        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCombatUi), nameof(NCombatUi.AnimOut))];
        }

        public static void Postfix(NCombatUi __instance)
        {
            if (MaterialInventoryHudPatches.TryGetHud(__instance, out var hud)) hud.Unbind();
        }
    }

    internal sealed class NCombatUiDeactivateMaterialInventoryHudPatch : IPatchMethod
    {
        public static string PatchId => "winefox_material_inventory_hud_combat_ui_deactivate";
        public static string Description => "Hide WineFox material inventory HUD alongside NCombatUi.Deactivate";
        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NCombatUi), nameof(NCombatUi.Deactivate))];
        }

        public static void Postfix(NCombatUi __instance)
        {
            if (MaterialInventoryHudPatches.TryGetHud(__instance, out var hud)) hud.Unbind();
        }
    }

    internal static class MaterialInventoryHudPatches
    {
        public static bool TryGetHud(NCombatUi combatUi, out NMaterialInventoryHud hud)
        {
            ModNodeAttachmentRegistry.EnsureReadyAttachments(combatUi);
            return ModNodeAttachmentRegistry.For(Const.ModId)
                .TryGetAttached<NCombatUi, NMaterialInventoryHud>(
                    combatUi,
                    NMaterialInventoryHud.AttachmentId,
                    out hud);
        }

        public static bool ShouldShowMaterialInventory(Player player)
        {
            return IsWineFox(player) || HasAnyMaterial(player);
        }

        public static bool IsWineFox(Player player)
        {
            return player.Character is WineFox ||
                   player.Character.Id.Entry.Contains("winefox", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasAnyMaterial(Player player)
        {
            return player.Creature.Powers.Any(power =>
                power is WoodPower or StonePower or IronPower or DiamondPower &&
                power.Amount > 0m);
        }
    }
}
