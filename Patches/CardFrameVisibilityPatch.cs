using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;
using STS2_WineFox.Cards.WineFoxCard;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    internal sealed class CardFrameVisibilityPatch : IPatchMethod
    {
        public static string PatchId => "winefox_card_frame_visibility";

        public static string Description =>
            "Let WineFox cards hide the card frame, title banner and type plaque individually";

        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets() =>
        [
            new(typeof(NCard), "Reload", null, true),
            new(typeof(NCard), "UpdateTypePlaque", null, true),
        ];

        public static void Postfix(NCard __instance)
        {
            try
            {
                Apply(__instance);
            }
            catch
            {
            }
        }

        private static void Apply(NCard card)
        {
            if (!GodotObject.IsInstanceValid(card) || card.Model is not WineFoxCardBase fox)
                return;

            if (!fox.ShowsCardFrame)
            {
                Hide(card, "%Frame");
                Hide(card, "%AncientBorder");
            }

            if (!fox.ShowsTitleBanner)
            {
                Hide(card, "%TitleBanner");
                Hide(card, "%AncientBanner");
            }

            if (!fox.ShowsTypePlaque)
            {
                Hide(card, "%TypePlaque");
                Hide(card, "%TypeLabel");
            }
        }

        private static void Hide(NCard card, string path)
        {
            if (card.GetNodeOrNull<CanvasItem>(path) is { } node)
                node.Visible = false;
        }
    }
}
