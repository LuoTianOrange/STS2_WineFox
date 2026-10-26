using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Models.Capabilities;

namespace STS2_WineFox.Cards.Spell
{
    /// <summary>
    ///     法术牌基类。
    ///     <para>
    ///         法术 = 可以装填进法杖、由回合结束的释放步骤结算的牌。
    ///     </para>
    ///     <para>
    ///         目标由 <see cref="TargetType" /> 按 Traits 派生：单体攻击法术要求玩家在装填时
    ///         选定敌人，目标随快照传递到释放阶段；群体、自身向与辅助向法术仍取 <c>None</c>。
    ///     </para>
    /// </summary>
    public abstract class MagicWineFoxSpellCard(
        int baseCost,
        CardType type,
        CardRarity rarity,
        TargetType target,
        bool showInCardLibrary = true)
        : WineFoxCard(baseCost, type, rarity, target, showInCardLibrary), IMagicWineFoxSpellIconProvider,
            IModelCapabilitySource
    {
        /// <summary>
        ///     单体攻击法术装填时需要选定目标，其余情况沿用构造传入的目标类型。
        ///     修正符不施放，始终沿用构造传入值。
        /// </summary>
        public override TargetType TargetType
        {
            get
            {
                if (this is IMagicWineFoxSpellModifierCard)
                    return base.TargetType;

                var traits = Traits;

                if (traits.TargetSelf || traits.AreaOfEffect || traits.RandomTarget)
                    return base.TargetType;

                return traits.AttackSpell ? TargetType.AnyEnemy : base.TargetType;
            }
        }

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
        ///     法术的通用属性声明。默认单体、单段、攻击型、可装填、接受修正符。
        /// </summary>
        public virtual SpellTraits Traits => SpellTraits.Default;

        /// <summary>能否装填进法杖，取自 Traits。</summary>
        public bool IsLoadable => Traits.Loadable;

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

        public decimal? PreviewBlockOverride { get; private set; }

        public bool HasBlockVar => DynamicVars.TryGetValue("Block", out _);

        /// <summary>
        ///     法杖槽位预览条中显示的小图标。
        ///     子类覆盖为自己的图标；返回空字符串时预览条回落到空槽占位图。
        /// </summary>
        public virtual string SpellIconPath => string.Empty;

        /// <summary>
        ///     法术槽已满时不能打出：装填会失败，卡牌白掉。
        /// </summary>
        protected override bool IsPlayable
        {
            get
            {
                if (MagicWineFoxSpellCmd.GetPower(Owner) is not { } power)
                    return true;

                return power.LoadedCount < power.SlotCapacity;
            }
        }

        /// <summary>
        ///     本场战斗允许的装填次数。
        ///     <para>
        ///         优先取 <see cref="SpellTraits.UsesLimit" /> 声明的上限；
        ///         未声明时按关键字推导：带消耗的牌只能用一次，其余视为无限。
        ///     </para>
        /// </summary>
        public int? UsesLimit =>
            Traits.UsesLimit ?? (Keywords.Contains(CardKeyword.Exhaust) ? 1 : null);

        [SavedProperty] public int UsesUsed { get; set; }

        /// <summary>是否处于无限使用状态（未声明上限，或遗物授予无限使用）。</summary>
        public bool IsUnlimitedUses =>
            UsesLimit == null ||
            (IsMutable &&
             Owner?.Relics?.OfType<ILimitedUseModifier>()
                 .Any(modifier => modifier.GrantsUnlimitedUses(this)) == true);

        /// <summary>剩余可用次数；无限使用时返回 <see cref="int.MaxValue" />。</summary>
        public int UsesRemaining =>
            IsUnlimitedUses ? int.MaxValue : Math.Max(0, UsesLimit!.Value - UsesUsed);

        /// <summary>消耗一次使用次数，返回是否因此用尽。</summary>
        public bool TryConsumeUse()
        {
            if (IsUnlimitedUses)
                return false;

            if (UsesUsed >= UsesLimit!.Value)
                return true;

            UsesUsed += 1;
            return UsesUsed >= UsesLimit.Value;
        }

        /// <summary>装填后结算使用次数：用尽时把这张牌送去消耗。</summary>
        protected async Task ConsumeUseAfterLoad(PlayerChoiceContext choiceContext)
        {
            if (TryConsumeUse())
                await CardCmd.Exhaust(choiceContext, this);
        }

        protected override void AddExtraArgsToDescription(LocString description)
        {
            base.AddExtraArgsToDescription(description);

            description.Add("Uses", IsUnlimitedUses ? "∞" : UsesRemaining.ToString());
        }

        void IModelCapabilitySource.BuildDefaultCapabilities(ModelCapabilityList capabilities)
        {
            capabilities.Add<SpellAttributeTipCapability>();
        }

        /// <summary>
        ///     按本次释放的法术总数计算的额外预览伤害，供条件加成类法术（如能量弹）参与估算。
        /// </summary>
        public virtual decimal PreviewExtraDamage(int spellCount)
        {
            return 0m;
        }

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

        /// <summary>记录计入加成后的单次格挡。</summary>
        public void ApplyPreviewBlock(decimal value)
        {
            PreviewBlockOverride = value;
        }

        public void ClearPreviewBlock()
        {
            PreviewBlockOverride = null;
        }

        public decimal BeginBlockDisplayOverride(decimal value)
        {
            if (!DynamicVars.TryGetValue("Block", out var block))
                return -1m;

            var trueBase = block.BaseValue;
            block.BaseValue = value;
            block.PreviewValue = value;
            return trueBase;
        }

        public void EndBlockDisplayOverride(decimal trueBase)
        {
            if (!DynamicVars.TryGetValue("Block", out var block))
                return;

            block.BaseValue = trueBase;
            block.PreviewValue = trueBase;
        }

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
        protected static Creature? ResolveSpellTarget(MagicWineFoxSpellCastContext context)
        {
            if (context.Target?.IsAlive == true)
                return context.Target;

            return context.Owner?.Creature?.CombatState?.HittableEnemies.FirstOrDefault();
        }

        protected static List<Creature> ResolveSpellTargets(MagicWineFoxSpellCastContext context)
        {
            if (context.TargetsSelf)
                return [];

            if (context.TargetsAllEnemies)
                return context.Owner?.Creature?.CombatState?.HittableEnemies
                    .Where(enemy => enemy.IsAlive)
                    .ToList() ?? [];

            return ResolveSpellTarget(context) is { } single ? [single] : [];
        }

        /// <summary>
        ///     结算法术伤害。基础段数来自 Traits 的 HitCount，随后追加额外段数。
        ///     <para>
        ///         额外段数乘在自家段数上，HitCount 为 3 的法术遇到三段额外段数时共 12 段。
        ///     </para>
        /// </summary>
        protected static async Task<IReadOnlyList<AttackCommand>> DealSpellDamage(
            MagicWineFoxSpellCastContext context,
            decimal baseDamage)
        {
            var commands = new List<AttackCommand>();
            var baseHits = BaseHitCount(context);

            for (var i = 0; i < baseHits; i++)
                await ExecuteHit(context, baseDamage, commands);

            commands.AddRange(await DealExtraDamageStrikes(context, baseDamage));
            return commands;
        }

        /// <summary>
        ///     追加修正符带来的额外段数。每份额外段数按 Traits 的 HitCount 放大。
        /// </summary>
        protected static async Task<IReadOnlyList<AttackCommand>> DealExtraDamageStrikes(
            MagicWineFoxSpellCastContext context,
            decimal baseDamage)
        {
            var commands = new List<AttackCommand>();
            var extraHits = ExtraHitCount(context);

            for (var i = 0; i < extraHits; i++)
                await ExecuteHit(context, baseDamage, commands);

            return commands;
        }

        /// <summary>本次释放的基础段数，取自 Traits 的 HitCount。</summary>
        protected static int BaseHitCount(MagicWineFoxSpellCastContext context)
        {
            return Math.Max(0, ResolveTraits(context).HitCount);
        }

        /// <summary>本次释放的额外段数。不吃额外段数的法术返回 0。</summary>
        protected static int ExtraHitCount(MagicWineFoxSpellCastContext context)
        {
            var traits = ResolveTraits(context);

            if (!traits.AcceptsExtraStrikes)
                return 0;

            return Math.Max(0, traits.HitCount) * Math.Max(0, context.ExtraDamageStrikes);
        }

        /// <summary>本次释放的总段数，供预览与伤害估算使用。</summary>
        protected static int ResolveHitCount(MagicWineFoxSpellCastContext context)
        {
            return BaseHitCount(context) + ExtraHitCount(context);
        }

        private static SpellTraits ResolveTraits(MagicWineFoxSpellCastContext context)
        {
            return (context.SourceCard as MagicWineFoxSpellCard)?.Traits ?? SpellTraits.Default;
        }

        private static async Task ExecuteHit(
            MagicWineFoxSpellCastContext context,
            decimal baseDamage,
            List<AttackCommand> commands)
        {
            if (context.IgnoresBlock)
            {
                await DealUnblockableDamage(context, context.DamageWithModifiers(baseDamage));
                return;
            }

            var attack = DamageCmd.Attack(context.DamageWithModifiers(baseDamage))
                .FromCard(context.SourceCard, null);

            if (context.TargetsAllEnemies)
            {
                if (context.Owner?.Creature?.CombatState is { } combatState)
                {
                    commands.Add(await attack.TargetingAllOpponents(combatState)
                        .Execute(context.ChoiceContext));
                }

                return;
            }

            var target = context.RandomTargets
                ? PickRandomEnemy(context.Owner)
                : ResolveSpellTarget(context);

            if (target != null)
            {
                commands.Add(await attack.Targeting(target).Execute(context.ChoiceContext));
            }
        }

        private static async Task DealUnblockableDamage(
            MagicWineFoxSpellCastContext context,
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
