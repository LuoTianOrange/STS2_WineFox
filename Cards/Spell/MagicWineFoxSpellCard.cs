using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
    ///     <para>
    ///         法术牌的 <see cref="TargetType" /> 一律为 <c>None</c>：它们被装填进法杖后
    ///         由铸法释放，运行时目标来自装填时的快照；直接打出时也无需玩家点选。
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
        /// <summary>
        ///     卡牌关键字：可装填时带「装填」+「释放」，构成法杖体系的一对；
        ///     不可装填时两者都不带（如奥术屏障，只直接释放）。
        /// </summary>
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            IsLoadable
                ? [WineFoxKeywords.LoadKeyword, WineFoxKeywords.ReleaseKeyword]
                : [];

        /// <summary>
        ///     能否装填进法杖。部分法术（如防御类）设计为只能直接释放，
        ///     此时返回 false；卡面只写自身效果，不写「不可装填」。
        /// </summary>
        public virtual bool IsLoadable => true;

        /// <summary>
        ///     释放时的目标回落：优先用快照记录的目标，其次回落到场上第一个可命中敌人。
        ///     <see cref="TargetType.None" /> 的法术在直接打出时没有 <c>play.Target</c>，
        ///     因此必须提供回落，否则单目标伤害会静默失效。
        /// </summary>
        protected static Creature? ResolveSpellTarget(Mechanics.MagicWineFoxSpellCastContext context)
        {
            if (context.Target?.IsAlive == true)
                return context.Target;

            return context.Owner?.Creature?.CombatState?.HittableEnemies.FirstOrDefault();
        }
    }
}
