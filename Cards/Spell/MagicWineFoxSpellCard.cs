using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Mechanics;

namespace STS2_WineFox.Cards.Spell
{
    /// <summary>
    ///     法术牌基类。
    ///     <para>
    ///         法术 = 可以装填进法杖、由回合结束的释放步骤结算的牌。
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
        ///     可装填时带装填；
        ///     实现 <see cref="IMagicWineFoxSpellModifierCard" /> 的修正符自动带法术修正。
        ///     <para>
        ///         释放关键字不在这里：它属于主动触发释放的牌（如定向爆破），
        ///         见 <c>DirectionalBlasting</c>。
        ///     </para>
        /// </summary>
        public override IEnumerable<CardKeyword> CanonicalKeywords
        {
            get
            {
                if (IsLoadable)
                    yield return WineFoxKeywords.LoadKeyword;

                if (this is IMagicWineFoxSpellModifierCard)
                {
                    yield return WineFoxKeywords.SpellModifierKeyword;
                    yield return CardKeyword.Ethereal;
                    yield return CardKeyword.Exhaust;
                }
            }
        }

        /// <summary>
        ///     能否装填进法杖。部分法术（如防御类）设计为只能直接释放，
        ///     此时返回 false；卡面只写自身效果，不写不可装填。
        /// </summary>
        public virtual bool IsLoadable => true;

        /// <summary>
        ///     本法术是否作用于敌人。用于是否攻击到敌人的判定
        ///     自身向法术应该返回 false。
        ///     <para>
        ///         由卡牌自己声明。
        ///     </para>
        /// </summary>
        public virtual bool TargetsEnemy => true;

        /// <summary>
        ///     预览用的单次基础伤害（不参与结算）。无伤害的法术保持 0。
        ///     供 <c>MagicWineFoxSpellCmd.PreviewRelease</c> 估算总伤害。
        /// </summary>
        public virtual decimal PreviewDamage => 0m;

        /// <summary>
        ///     预览用的基础格挡（不参与结算）。无格挡的法术保持 0。
        ///     <para>
        ///         供 <c>MagicWineFoxSpellCmd.RefreshWandCardValues</c> 把
        ///         <c>IMagicBlockModifier</c>（如【压缩施法】）的加成算进卡面显示。
        ///     </para>
        /// </summary>
        public virtual decimal PreviewBlock => 0m;

        /// <summary>
        ///     计入法术修正符后的单次伤害（null = 未计算，按原版显示）。
        ///     由 <c>MagicWineFoxSpellCmd.RefreshWandCardValues</c> 写入，
        ///     描述渲染时由 <c>SpellCardDescriptionPreviewPatch</c> 临时换进变量。
        /// </summary>
        public decimal? PreviewDamageOverride { get; private set; }

        /// <summary>记录计入修正符后的单次伤害。</summary>
        public void ApplyPreviewDamage(decimal value)
        {
            PreviewDamageOverride = value;
        }

        /// <summary>清掉覆盖，恢复原版显示。</summary>
        public void ClearPreviewDamage()
        {
            PreviewDamageOverride = null;
        }

        /// <summary>
        ///     计入法术修正符与 <c>IMagicBlockModifier</c> 后的单次格挡（null = 未计算）。
        ///     与 <see cref="PreviewDamageOverride" /> 同一套机制，供描述渲染时临时换值。
        /// </summary>
        public decimal? PreviewBlockOverride { get; private set; }

        /// <summary>记录计入加成后的单次格挡。</summary>
        public void ApplyPreviewBlock(decimal value)
        {
            PreviewBlockOverride = value;
        }

        /// <summary>清掉格挡覆盖，恢复原版显示。</summary>
        public void ClearPreviewBlock()
        {
            PreviewBlockOverride = null;
        }

        /// <summary>
        ///     这张法术是否带格挡变量。没有格挡的法术（如【狐火弹】只有伤害）不能走格挡覆盖，
        ///     否则 <c>DynamicVars["Block"]</c> 会抛 KeyNotFoundException。
        /// </summary>
        public bool HasBlockVar => DynamicVars.TryGetValue("Block", out _);

        /// <summary>描述渲染开始：临时把 <c>Block</c> 变量换成该值，返回渲染前的真实值供还原。</summary>
        public decimal BeginBlockDisplayOverride(decimal value)
        {
            if (!DynamicVars.TryGetValue("Block", out var block))
                return -1m;

            var trueBase = block.BaseValue;
            block.BaseValue = value;
            block.PreviewValue = value;
            return trueBase;
        }

        /// <summary>描述渲染结束：把格挡变量还原成真实基础值。</summary>
        public void EndBlockDisplayOverride(decimal trueBase)
        {
            if (!DynamicVars.TryGetValue("Block", out var block))
                return;

            block.BaseValue = trueBase;
            block.PreviewValue = trueBase;
        }

        /// <summary>
        ///     描述渲染开始：临时把 <c>Damage</c> 变量换成该值，返回渲染前的真实值供还原。
        ///     返回 <c>-1</c> 表示无需还原。
        /// </summary>
        public decimal BeginDisplayOverride(decimal value)
        {
            if (!DynamicVars.TryGetValue("Damage", out var damage))
                return -1m;

            var trueBase = damage.BaseValue;
            damage.BaseValue = value;
            damage.PreviewValue = value;
            return trueBase;
        }

        /// <summary>描述渲染结束：把变量还原成真实基础值。</summary>
        public void EndDisplayOverride(decimal trueBase)
        {
            if (!DynamicVars.TryGetValue("Damage", out var damage))
                return;

            damage.BaseValue = trueBase;
            damage.PreviewValue = trueBase;
        }

        /// <summary>
        ///     释放时的目标回落：优先用快照记录的目标，其次回落到场上第一个可命中敌人。
        ///     <para>
        ///         <see cref="TargetType.None" /> 的法术在直接打出时没有 <c>play.Target</c>，
        ///         因此必须提供回落，否则单目标伤害会静默失效。
        ///     </para>
        /// </summary>
        protected static Creature? ResolveSpellTarget(Mechanics.MagicWineFoxSpellCastContext context)
        {
            if (context.Target?.IsAlive == true)
                return context.Target;

            return context.Owner?.Creature?.CombatState?.HittableEnemies.FirstOrDefault();
        }

        protected static List<Creature> ResolveSpellTargets(Mechanics.MagicWineFoxSpellCastContext context)
        {
            if (context.TargetsAllEnemies)
                return context.Owner?.Creature?.CombatState?.HittableEnemies
                    .Where(enemy => enemy.IsAlive)
                    .ToList() ?? [];

            return ResolveSpellTarget(context) is { } single ? [single] : [];
        }

        protected static async Task<IReadOnlyList<AttackCommand>> DealSpellDamage(
            Mechanics.MagicWineFoxSpellCastContext context,
            decimal baseDamage)
        {
            var commands = new List<AttackCommand>();

            if (context.IgnoresBlock)
            {
                await DealUnblockableDamage(context, context.DamageWithModifiers(baseDamage));
                commands.AddRange(await DealExtraDamageStrikes(context, baseDamage));
                return commands;
            }

            var attack = DamageCmd.Attack(context.DamageWithModifiers(baseDamage))
                .FromCard(context.SourceCard, null);

            if (context.TargetsAllEnemies)
            {
                if (context.Owner?.Creature?.CombatState is { } combatState)
                {
                    commands.Add(await attack.TargetingAllOpponents(combatState).Execute(context.ChoiceContext));
                }
            }
            else
            {
                var target = ResolveSpellTarget(context);
                if (target != null)
                {
                    commands.Add(await attack.Targeting(target).Execute(context.ChoiceContext));
                }
            }

            commands.AddRange(await DealExtraDamageStrikes(context, baseDamage));
            return commands;
        }

        protected static async Task<IReadOnlyList<AttackCommand>> DealExtraDamageStrikes(
            Mechanics.MagicWineFoxSpellCastContext context,
            decimal baseDamage)
        {
            var commands = new List<AttackCommand>();
            var strikes = context.ExtraDamageStrikes;

            for (var i = 0; i < strikes; i++)
            {
                if (context.IgnoresBlock)
                {
                    await DealUnblockableDamage(context, context.DamageWithModifiers(baseDamage));
                    continue;
                }

                var strike = DamageCmd.Attack(context.DamageWithModifiers(baseDamage))
                    .FromCard(context.SourceCard, null);

                if (context.TargetsAllEnemies)
                {
                    if (context.Owner?.Creature?.CombatState is { } combatState)
                    {
                        commands.Add(await strike.TargetingAllOpponents(combatState).Execute(context.ChoiceContext));
                    }

                    continue;
                }

                var target = context.RandomTargets
                    ? PickRandomEnemy(context.Owner)
                    : ResolveSpellTarget(context);

                if (target != null)
                {
                    commands.Add(await strike.Targeting(target).Execute(context.ChoiceContext));
                }
            }

            return commands;
        }

        private static async Task DealUnblockableDamage(
            Mechanics.MagicWineFoxSpellCastContext context,
            decimal damage)
        {
            const ValueProp props = ValueProp.Unblockable | ValueProp.Move;

            if (context.TargetsAllEnemies)
            {
                if (context.Owner?.Creature?.CombatState is { } combatState)
                {
                    foreach (var enemy in combatState.HittableEnemies.ToList())
                    {
                        await CreatureCmd.Damage(
                            context.ChoiceContext, enemy, damage, props, context.SourceCard, null);
                    }
                }

                return;
            }

            var target = ResolveSpellTarget(context);
            if (target != null)
            {
                await CreatureCmd.Damage(
                    context.ChoiceContext, target, damage, props, context.SourceCard, null);
            }
        }

        private static Creature? PickRandomEnemy(Player? owner)
        {
            var enemies = owner?.Creature?.CombatState?.HittableEnemies;

            if (enemies == null || enemies.Count == 0)
                return null;

            return owner!.Creature!.CombatState!.RunState.Rng.CombatTargets.NextItem(enemies);
        }
    }
}
