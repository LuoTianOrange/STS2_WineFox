using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using STS2_WineFox.Mechanics;

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
        : WineFoxCard(baseCost, type, rarity, target, showInCardLibrary), IMagicWineFoxSpellIconProvider
    {
        /// <summary>
        ///     法杖槽位预览条中显示的小图标。
        ///     子类覆盖为自己的图标；返回空字符串时预览条回落到空槽占位图。
        /// </summary>
        public virtual string SpellIconPath => string.Empty;

        /// <summary>
        ///     卡牌关键字：
        ///     可装填时带「装填」+「释放」，构成法杖体系的一对（不可装填时两者都不带）；
        ///     实现 <see cref="IMagicWineFoxSpellModifierCard" /> 的修正符自动带「法术修正」。
        ///     这样新增修正符时不必再手工维护关键字列表。
        /// </summary>
        public override IEnumerable<CardKeyword> CanonicalKeywords
        {
            get
            {
                if (IsLoadable)
                {
                    yield return WineFoxKeywords.LoadKeyword;
                    yield return WineFoxKeywords.ReleaseKeyword;
                }

                if (this is IMagicWineFoxSpellModifierCard)
                    yield return WineFoxKeywords.SpellModifierKeyword;
            }
        }

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

        /// <summary>
        ///     法术伤害的统一结算入口。
        ///     <para>
        ///         由修正符决定目标：带【穿刺魔弹】这类「下一个法术对所有敌人造成伤害」的修正时，
        ///         改为全体结算；否则按 <see cref="ResolveSpellTarget" /> 打单体。
        ///         子类因此不必各自处理目标逻辑。
        ///     </para>
        /// </summary>
        protected static async Task DealSpellDamage(
            Mechanics.MagicWineFoxSpellCastContext context,
            decimal baseDamage)
        {
            var attack = DamageCmd.Attack(context.DamageWithModifiers(baseDamage))
                .FromCard(context.SourceCard, null);

            if (context.TargetsAllEnemies)
            {
                if (context.Owner?.Creature?.CombatState is { } combatState)
                    await attack.TargetingAllOpponents(combatState).Execute(context.ChoiceContext);
                return;
            }

            var target = ResolveSpellTarget(context);
            if (target != null)
                await attack.Targeting(target).Execute(context.ChoiceContext);
        }
    }
}
