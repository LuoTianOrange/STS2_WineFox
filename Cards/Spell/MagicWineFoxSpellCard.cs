using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    /// <summary>
    ///     法术牌基类。
    ///     <para>
    ///         「法术」= 可以装填进法杖、由回合结束的释放步骤结算的牌。
    ///         与既有 <c>magic</c>（咏唱/魔法管线）刻意分开：法术体系的强度来自
    ///         「释放轮数 + 槽位 + 修正符」，不参与 <c>MagicDamage</c> / <c>ChantPower</c> 的结算。
    ///     </para>
    /// </summary>
    public abstract class MagicWineFoxSpellCard(
        int baseCost,
        CardType type,
        CardRarity rarity,
        TargetType target,
        bool showInCardLibrary = true)
        : WineFoxCard(baseCost, type, rarity, target, showInCardLibrary)
    {
        /// <summary>卡牌是否带「法术」关键字。装填与施法链只认这个关键字。</summary>
        public sealed override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.SpellKeyword];

        /// <summary>
        ///     能否装填进法杖。部分法术（如防御类）设计为只能直接释放，
        ///     此时返回 false，并在卡面写明（v0.2.6「不可装填」）。
        /// </summary>
        public virtual bool IsLoadable => true;
    }
}
