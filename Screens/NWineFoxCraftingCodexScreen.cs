using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using STS2_WineFox.Cards;
using STS2_WineFox.Cards.Token.HellGift;
using STS2_WineFox.Cards.Token.LessHoliday;

namespace STS2_WineFox.Screens
{
    /// <summary>
    ///     WineFox 合成表界面。
    ///     <para>
    ///         本实现完全 <b>复用原版 <see cref="NCardPileScreen" /> 场景</b>（即"抽牌堆 / 弃牌堆 / 消耗堆"
    ///         点开后看到的那个卡牌网格视图）——不再自己摆排版、画标题栏、塞标签、手搓 HFlowContainer。
    ///         这样做的好处：
    ///         <list type="bullet">
    ///             <item>卡牌渲染、滚动、返回按钮、背景蒙版、hover-tip 全部与原版一致</item>
    ///             <item>随原版 UI 风格升级自动同步，不会漂移</item>
    ///             <item>键鼠与手柄导航、退出快捷键等交互完全等同抽牌堆</item>
    ///         </list>
    ///     </para>
    ///     <para>
    ///         实现手法：我们构造一个不绑定任何战斗或角色的"合成 <see cref="CardPile" />"
    ///         （<see cref="PileType.None" />），把所有合成可获得的卡的 <i>mutable 副本</i> 塞进去，然后
    ///         直接调 <see cref="NCardPileScreen.ShowScreen" />。原版 <see cref="NCardPileScreen" /> 对
    ///         <see cref="PileType.None" /> 会默认隐藏底部说明条，我们在 <see cref="Open" /> 里再通过
    ///         <c>%BottomLabel</c> 唯一名把它重新显示并写成"合成表"文案。
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     "是否打开"状态通过比对 <see cref="NCapstoneContainer.Instance" /> 当前 capstone 的
    ///     <see cref="NCardPileScreen.Pile" /> 引用是否等于 <see cref="CurrentPile" /> 来判定——这避免了新增
    ///     screen 类型的需要，并且自然地与原版的切换 / 关闭逻辑一致（player 切别的 capstone 后
    ///     <c>CurrentPile</c> 会在下次打开时被覆盖）。
    /// </remarks>
    public static class NWineFoxCraftingCodexScreen
    {
        /// <summary>
        ///     记录最近一次通过 <see cref="Open" /> 构造并传给 <see cref="NCardPileScreen" /> 的合成 pile，
        ///     用于 <see cref="IsCurrentlyOpen" /> 快速判断当前 capstone 是不是"我们的"合成表。
        /// </summary>
        private static CardPile? CurrentPile { get; set; }

        /// <summary>
        ///     构造合成 pile 并以抽牌堆样式打开合成表界面。若同一合成表已经打开则直接返回既有实例
        ///     （幂等，方便切换动画后重复按按钮）。
        /// </summary>
        public static NCardPileScreen Open()
        {
            var pile = BuildSyntheticPile();
            CurrentPile = pile;

            var screen = NCardPileScreen.ShowScreen(pile, []);
            screen.Name = "NWineFoxCraftingCodexScreen";

            OverrideBottomLabel(screen);
            return screen;
        }

        /// <summary>
        ///     返回当前 capstone 是否正好是本合成表屏幕。实现细节：检查 capstone 容器里的
        ///     <see cref="NCardPileScreen" /> 的 <see cref="NCardPileScreen.Pile" /> 是不是我们上一次构造的
        ///     <see cref="CurrentPile" />。
        /// </summary>
        public static bool IsCurrentlyOpen()
        {
            if (CurrentPile == null)
                return false;
            var current = NCapstoneContainer.Instance?.CurrentCapstoneScreen;
            return current is NCardPileScreen pileScreen && ReferenceEquals(pileScreen.Pile, CurrentPile);
        }

        /// <summary>
        ///     合成表要显示的全部 <see cref="CardModel" />。包括：
        ///     <list type="bullet">
        ///         <item>所有在 <see cref="CraftRecipeRegistry.All" /> 里注册的产物</item>
        ///         <item>
        ///             来自 <c>LessHoliday</c> 的 <see cref="WorkWork" />、来自 <c>HellGift</c> 的
        ///             <see cref="GoldenSword" /> / <see cref="GoldenPickaxe" /> / <see cref="GoldenArmor" />
        ///             ——它们不在 recipe registry 里但同属"通过合成 / 合成衍生效果"获得，一并列入
        ///             方便玩家一眼看全合成体系的产物。
        ///         </item>
        ///     </list>
        ///     返回 <see cref="CardModel" /> 的 <see cref="CardModel.ToMutable" /> 拷贝，以便 pile 的
        ///     <see cref="CardPile.AddInternal" /> 通过 <c>AssertMutable()</c>。
        /// </summary>
        public static IEnumerable<CardModel> EnumerateCraftableCards()
        {
            var seen = new HashSet<ModelId>();

            foreach (var recipe in CraftRecipeRegistry.All)
                if (TryResolveCardModel(recipe.ProductCardType, out var card) && seen.Add(card.Id))
                    yield return card;

            foreach (var extra in new[]
                     {
                         typeof(WorkWork),
                         typeof(GoldenSword),
                         typeof(GoldenPickaxe),
                         typeof(GoldenArmor),
                     })
                if (TryResolveCardModel(extra, out var card) && seen.Add(card.Id))
                    yield return card;
        }

        private static CardPile BuildSyntheticPile()
        {
            // PileType.None keeps CardPile.IsCombatPile == false, so AddInternal doesn't try to subscribe
            // the card to CombatManager.StateTracker — exactly what we want for a view-only list.
            var pile = new CardPile(PileType.None);
            foreach (var card in EnumerateCraftableCards())
                pile.AddInternal(card, silent: true);
            return pile;
        }

        private static bool TryResolveCardModel(Type cardType, out CardModel card)
        {
            try
            {
                var canonical = ModelDb.GetById<CardModel>(ModelDb.GetId(cardType));
                card = canonical.ToMutable();
                return true;
            }
            catch
            {
                card = null!;
                return false;
            }
        }

        /// <summary>
        ///     原版 <see cref="NCardPileScreen" /> 对 <see cref="PileType.None" /> 会直接把底部说明条
        ///     <c>%BottomLabel</c> 隐藏（默认 switch 分支）。我们反向把它显示回来并写成合成表解释文案——
        ///     完整保留原版的字体 / 阴影 / 描边样式，只替换文本。
        /// </summary>
        private static void OverrideBottomLabel(NCardPileScreen screen)
        {
            try
            {
                var label = screen.GetNodeOrNull<MegaRichTextLabel>("%BottomLabel");
                if (label == null)
                    return;

                label.Visible = true;
                label.Text = "[center]"
                             + new LocString("static_hover_tips",
                                     "STS2_WINE_FOX_TOPBARBUTTON_CRAFTING_CODEX.screen_info")
                                 .GetFormattedText();
            }
            catch
            {
                // Non-fatal — the bottom label is purely informational, so a missing node or layout quirk
                // shouldn't take the whole screen down.
            }
        }
    }
}
