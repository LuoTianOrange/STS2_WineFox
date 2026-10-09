using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2_WineFox.Relics;
using STS2RitsuLib;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    /// <summary>
    ///     悬停初始遗物（<see cref="MagicWineFoxWand" />）时，在提示里追加：
    ///     一行伤害：xxx文字 + 法杖内法术的卡牌预览（按执行顺序）。
    ///     <para>
    ///         全部走 RitsuLib 的公开助手（<see cref="HoverTipHelper" />），
    ///         由它内部去写悬停集合的容器，因此不需要反射、也不需要自己碰私有字段。
    ///     </para>
    ///     <para>
    ///         <c>NHoverTipSet.CreateAndShow</c> 有重载，只给方法名会因歧义导致补丁失败，
    ///         所以这里显式列出参数类型；不存在的重载由 <c>IgnoreIfMissing</c> 安全跳过。
    ///     </para>
    /// </summary>
    internal sealed class RelicHoverSpellOrderPatch : IPatchMethod
    {
        public static string PatchId => "winefox_relic_hover_spell_order";
        public static string Description => "Append damage line and spell cards to the starter relic hover tip";
        public static bool IsCritical => false;

        private static readonly HashSet<Control> HandledThisFrame = [];
        private static ulong _frame;

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(typeof(NHoverTipSet), "CreateAndShow",
                    [typeof(Control), typeof(IEnumerable<IHoverTip>), typeof(HoverTipAlignment)], true),
                new(typeof(NHoverTipSet), "CreateAndShow",
                    [typeof(Control), typeof(IHoverTip), typeof(HoverTipAlignment)], true),
            ];
        }

        public static void Postfix(NHoverTipSet __result, object[] __args)
        {
            if (__result == null || !GodotObject.IsInstanceValid(__result))
                return;

            var owner = FindOwner(__args);
            if (owner == null || !GodotObject.IsInstanceValid(owner))
                return;

            if (!IsStarterRelicTip(__args))
                return;

            if (!ShouldHandle(owner))
                return;

            var power = ResolvePower();
            if (power == null)
                return;

            var preview = MagicWineFoxSpellCmd.PreviewRelease(power);
            if (preview.Segments.Count == 0)
                return;

            var damageLine = new LocString("relics", "STS2_WINE_FOX_RELIC_MAGIC_WINE_FOX_WAND.ORDER_DAMAGE");
            damageLine.Add("Damage", preview.TotalDamage.ToString("0"));

            // 标题放「伤害：xxx」，正文放分段后的图标行——于是伤害显示在顺序行上方。
            // 正文是 MegaRichTextLabel，用 Godot 的 [img] 标签绘制法术图标与箭头。
            HoverTipHelper.AddTipToOwner(owner, damageLine.GetFormattedText(), BuildOrderRows(preview));
        }

        private const float CastIconSize = 36f;
        private const float ModifierIconSize = 26f;
        private const float ArrowWidth = 24f;
        private const float ArrowHeight = 16f;

        /// <summary>
        ///     每段一行：正向结算一段，逆遍历后的倒序重放另起一行。
        ///     重放行前置↩、保留行前置…，用符号区分而不是文字，避免行内变长。
        /// </summary>
        private static string BuildOrderRows(MagicWineFoxSpellReleasePreview preview)
        {
            var rows = new List<string>();

            foreach (var segment in preview.Segments)
            {
                var parts = new List<string>();

                if (segment.Kind is MagicWineFoxSpellPreviewSegmentKind.Replay)
                    parts.Add("↪ ");
                else if (segment.Kind is MagicWineFoxSpellPreviewSegmentKind.Kept)
                    parts.Add("… ");

                var first = true;

                foreach (var step in segment.Steps)
                {
                    if (!first)
                        parts.Add(ImgTag(Const.Paths.SpellOrderArrow, ArrowWidth, ArrowHeight));

                    first = false;

                    var icon = step.Card is IMagicWineFoxSpellIconProvider provider
                        ? provider.SpellIconPath
                        : string.Empty;

                    var size = step.Kind is MagicWineFoxSpellPreviewKind.Modifier
                        ? ModifierIconSize
                        : CastIconSize;

                    if (!string.IsNullOrEmpty(icon))
                        parts.Add(ImgTag(icon, size, size));
                }

                rows.Add(string.Join("", parts));
            }

            return string.Join("\n", rows);
        }

        private static string ImgTag(string path, float width, float height)
        {
            return $"[img={width:0}x{height:0}]{path}[/img]";
        }

        /// <summary>同一帧内同一个控件只处理一次——两个重载可能先后触发。</summary>
        private static bool ShouldHandle(Control owner)
        {
            var frame = Engine.GetProcessFrames();
            if (frame != _frame)
            {
                _frame = frame;
                HandledThisFrame.Clear();
            }

            return HandledThisFrame.Add(owner);
        }

        private static Control? FindOwner(object[] args)
        {
            foreach (var arg in args)
            {
                if (arg is Control control)
                    return control;
            }

            return null;
        }

        private static bool IsStarterRelicTip(object[] args)
        {
            foreach (var arg in args)
            {
                if (arg is IHoverTip single && single.CanonicalModel is MagicWineFoxWand)
                    return true;

                if (arg is not IEnumerable<IHoverTip> tips)
                    continue;

                foreach (var tip in tips)
                {
                    if (tip.CanonicalModel is MagicWineFoxWand)
                        return true;
                }
            }

            return false;
        }

        private static MagicWineFoxSpellSlotPower? ResolvePower()
        {
            var state = CombatManager.Instance?.DebugOnlyGetState();
            var player = state == null ? null : LocalContext.GetMe(state);
            return player == null ? null : MagicWineFoxSpellCmd.GetPower(player);
        }
    }
}
