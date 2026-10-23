using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Potions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2_WineFox.Potions;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    public sealed class WineFoxMerchantSellablePotionTargetingPatch : IPatchMethod
    {
        public static string PatchId => "winefox_sellable_potion_merchant_targeting";
        public static bool IsCritical => true;
        public static string Description => "Focus merchant button when selling potions";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(NPotionHolder), "TargetNode")];
        }

        public static void Prefix(NPotionHolder __instance, TargetType targetType, ref NMerchantButton? __state)
        {
            if (targetType != TargetType.TargetedNoCreature)
                return;

            if (__instance.Potion?.Model is not SellableToMerchantPotionModel)
                return;

            var button = NMerchantRoom.Instance?.MerchantButton;
            if (button == null)
                return;

            __state = button;
            button.SetFocusMode(Control.FocusModeEnum.All);
            button.TryGrabFocus();
        }

        public static void Postfix(NMerchantButton? __state)
        {
            __state?.SetFocusMode(Control.FocusModeEnum.None);
        }
    }
}
