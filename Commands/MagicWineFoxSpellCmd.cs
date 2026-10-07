using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Commands
{
    /// <summary>
    ///     法杖/法术系统的唯一入口命令。
    ///     <para>
    ///         结算模型：
    ///         装填 = 支付卡面费用后把法术放进槽位；释放 = 回合结束按槽位顺序释放，
    ///         修正符累积、遇到法术后消费并清空。
    ///     </para>
    /// </summary>
    public static class MagicWineFoxSpellCmd
    {
        /// <summary>默认槽位容量。</summary>
        public const int DefaultCapacity = 4;

        /// <summary>默认释放轮数。</summary>
        public const int DefaultCastCount = 1;

        /// <summary>起始遗物：每回合第一次装填减免的费用。</summary>
        public const int FirstLoadDiscount = 1;

        public static async Task<MagicWineFoxSpellSlotPower?> EnsurePower(Player owner, int capacity = DefaultCapacity,
            int castCount = DefaultCastCount)
        {
            if (owner?.Creature == null) return null;

            var existing = owner.Creature.Powers.OfType<MagicWineFoxSpellSlotPower>().FirstOrDefault();
            if (existing != null)
            {
                existing.EnsureCapacity(Math.Max(existing.SlotCapacity, capacity));
                return existing;
            }

            var applied = await PowerCmd.Apply<MagicWineFoxSpellSlotPower>(
                new ThrowingPlayerChoiceContext(),
                owner.Creature,
                Math.Clamp(capacity, 0, 12),
                owner.Creature,
                null);

            if (applied == null) return null;

            applied.SetCastCount(castCount);
            return applied;
        }

        public static MagicWineFoxSpellSlotPower? GetPower(Player owner)
        {
            return owner?.Creature?.Powers.OfType<MagicWineFoxSpellSlotPower>().FirstOrDefault();
        }

        /// <summary>装填一张法术。返回是否成功（槽满时失败）。</summary>
        public static async Task<bool> Load(
            PlayerChoiceContext choiceContext,
            CardModel card,
            CardPlay play,
            bool isModifier = false,
            int xValue = 0)
        {
            var owner = card.Owner;
            var power = await EnsurePower(owner);
            if (power == null) return false;

            var snapshot = new MagicWineFoxSpellSlotSnapshot(card.CreateClone(), play.Target, isModifier, xValue);

            // 序列回响：把这张法术记为「始终释放」的法术。
            var echo = owner?.Creature?.Powers.OfType<Powers.EchoesSequencePower>().FirstOrDefault();
            echo?.TryBind(card);

            // 槽满：按设计「装不下就是装不下」，不做自动过载（充能球式的槽满转化不适用于法杖）。
            if (!power.TryLoad(snapshot)) return false;
            
            if (power.CanDiscountFirstLoad)
                power.ConsumeFirstLoadDiscount();

            // 装填后立刻刷新法杖内所有卡的显示数值（Power + 法术修正符）。
            RefreshWandCardValues(power);

            return true;
        }

        /// <summary>释放：按装填顺序释放全部已装填法术。</summary>
        public static async Task CastAll(
            PlayerChoiceContext choiceContext,
            Player owner,
            Creature? fallbackTarget,
            CardModel? sourceCard)
        {
            var power = GetPower(owner);
            if (power == null) return;

            var spells = power.DrainLoadedSpells();

            var preserved = spells.Count > 0
                ? await ResolveSpellChain(choiceContext, owner, fallbackTarget, sourceCard, spells, power.ReleaseBudget)
                : [];
            
            foreach (var snapshot in preserved)
                if (!power.TryLoad(snapshot))
                    break;
            
            EnsureEchoLoaded(owner);
            RefreshWandCardValues(power);
        }

        /// <summary>把【序列回响】绑定的法术自动装回法杖。</summary>
        private static void EnsureEchoLoaded(Player owner)
        {
            var echo = owner?.Creature?.Powers.OfType<Powers.EchoesSequencePower>().FirstOrDefault();
            if (echo?.EchoSpell is not { } template)
                return;

            var power = GetPower(owner);
            if (power == null)
                return;

            // 每次装回都用新的克隆，避免复用已被结算过的实例。
            power.TryLoad(new MagicWineFoxSpellSlotSnapshot(template.CreateClone(), null, false));
        }

        /// <summary>释放序列中最旧的一张法术，其余保留。</summary>
        public static async Task CastOldest(
            PlayerChoiceContext choiceContext,
            Player owner,
            Creature? fallbackTarget,
            CardModel? sourceCard)
        {
            var power = GetPower(owner);
            if (power == null) return;

            var oldest = power.DrainOldest();
            if (oldest == null) return;

            var preserved = await ResolveSpellChain(
                choiceContext,
                owner,
                fallbackTarget,
                sourceCard,
                [oldest],
                1);

            foreach (var snapshot in preserved)
                if (!power.TryLoad(snapshot))
                    break;
        }

        public static void ClearLoaded(Player owner)
        {
            GetPower(owner)?.ClearSlots();
        }

        /// <summary>
        ///     顺序释放：累积修正 → 遇法术按次数施放 → 清空累积器。
        /// </summary>
        private static async Task<IReadOnlyList<MagicWineFoxSpellSlotSnapshot>> ResolveSpellChain(
            PlayerChoiceContext choiceContext,
            Player owner,
            Creature? fallbackTarget,
            CardModel? sourceCard,
            IReadOnlyList<MagicWineFoxSpellSlotSnapshot> spells,
            int releaseBudget)
        {
            var modifiers = new MagicWineFoxSpellModifierState();
            var used = 0;
            var lookBehind = CollectLookBehindModifiers(spells);
            var history = new List<MagicWineFoxSpellSlotSnapshot>();

            modifiers.SetWandModifierCount(
                spells.Count(snapshot => snapshot.Card is IMagicWineFoxSpellModifierCard));
            modifiers.SetWandSpellCount(
                spells.Count(snapshot => snapshot.Card is IMagicWineFoxSpellCard
                    and not IMagicWineFoxSpellModifierCard));

            foreach (var snapshot in spells)
            {
                if (snapshot.Card is IMagicWineFoxSpellWandModifierCard wandModifier)
                {
                    modifiers.SetCurrentX(snapshot.XValue);
                    wandModifier.ApplyModifier(modifiers);
                }
            }

            for (var index = 0; index < spells.Count; index++)
            {
                var snapshot = spells[index];

                if (used >= releaseBudget)
                    break;

                if (snapshot.Card is IMagicWineFoxSpellReverseModifierCard)
                {
                    var replayState = modifiers.Copy();

                    for (var rewind = history.Count - 1; rewind >= 0; rewind--)
                    {
                        await CastSpellEntry(
                            choiceContext,
                            owner,
                            sourceCard,
                            fallbackTarget,
                            history[rewind],
                            replayState);

                        replayState = CreateReplayState(modifiers);
                    }

                    return spells.Skip(index + 1).ToList();
                }

                if (snapshot.Card is IMagicWineFoxSpellModifierCard modifierEntry)
                {
                    if (snapshot.Card is not IMagicWineFoxSpellLookBehindModifierCard and
                        not IMagicWineFoxSpellWandModifierCard)
                    {
                        modifiers.SetCurrentX(snapshot.XValue);
                        modifierEntry.ApplyModifier(modifiers);
                    }

                    if (modifiers.DrawForModifierEntries && modifiers.DrawCount > 0m)
                        await CardPileCmd.Draw(choiceContext, modifiers.DrawCount, owner);

                    continue;
                }

                if (snapshot.Card is not IMagicWineFoxSpellCard)
                {
                    modifiers.Reset();
                    continue;
                }

                if (lookBehind.TryGetValue(index, out var attached))
                    foreach (var extra in attached)
                    {
                        modifiers.SetCurrentX(extra.XValue);
                        (extra.Card as IMagicWineFoxSpellModifierCard)?.ApplyModifier(modifiers);
                    }

                history.Add(snapshot);

                await CastSpellEntry(choiceContext, owner, sourceCard, fallbackTarget, snapshot, modifiers);

                used += modifiers.BudgetCost;
                modifiers.Reset();
            }

            return [];
        }

        /// <summary>
        ///     预览本次释放：**与 <see cref="ResolveSpellChain" /> 使用同一套规则推演**，
        ///     返回执行顺序与预估总伤害，供 UI（如遗物悬停提示）显示。
        /// </summary>
        public static MagicWineFoxSpellReleasePreview PreviewRelease(MagicWineFoxSpellSlotPower? power)
        {
            var empty = new MagicWineFoxSpellReleasePreview([], 0m);
            if (power == null)
                return empty;

            var spells = power.Slots.Where(snapshot => snapshot != null).Select(snapshot => snapshot!).ToList();
            if (spells.Count == 0)
                return empty;

            var modifiers = new MagicWineFoxSpellModifierState();
            var used = 0;
            var lookBehind = CollectLookBehindModifiers(spells);
            var segments = new List<MagicWineFoxSpellPreviewSegment>();
            var forward = new List<MagicWineFoxSpellPreviewStep>();
            var pending = new List<CardModel>();
            var cast = new List<MagicWineFoxSpellSlotSnapshot>();
            var total = 0m;

            // 全体修正时，每次伤害会打到所有可命中敌人，总伤害需乘以敌人数。
            var enemyCount = Math.Max(1, power.Owner?.CombatState?.HittableEnemies.Count ?? 1);

            modifiers.SetWandModifierCount(
                spells.Count(snapshot => snapshot.Card is IMagicWineFoxSpellModifierCard));
            modifiers.SetWandSpellCount(
                spells.Count(snapshot => snapshot.Card is IMagicWineFoxSpellCard
                    and not IMagicWineFoxSpellModifierCard));

            foreach (var snapshot in spells)
            {
                if (snapshot.Card is IMagicWineFoxSpellWandModifierCard wandModifier)
                {
                    modifiers.SetCurrentX(snapshot.XValue);
                    wandModifier.ApplyModifier(modifiers);
                }
            }

            for (var index = 0; index < spells.Count; index++)
            {
                var snapshot = spells[index];

                if (used >= power.ReleaseBudget)
                    break;

                if (snapshot.Card is IMagicWineFoxSpellReverseModifierCard)
                {
                    forward.Add(new MagicWineFoxSpellPreviewStep(
                        snapshot.Card, MagicWineFoxSpellPreviewKind.Reverse, 0m, 0));
                    segments.Add(new MagicWineFoxSpellPreviewSegment(
                        MagicWineFoxSpellPreviewSegmentKind.Forward, forward));

                    var replay = new List<MagicWineFoxSpellPreviewStep>();
                    
                    for (var back = pending.Count - 1; back >= 0; back--)
                    {
                        replay.Add(new MagicWineFoxSpellPreviewStep(
                            pending[back], MagicWineFoxSpellPreviewKind.Modifier, 0m, 0));
                    }

                    var replayState = modifiers.Copy();

                    for (var rewind = cast.Count - 1; rewind >= 0; rewind--)
                    {
                        var (replayDamage, replayHits) = EstimateDamage(cast[rewind].Card, replayState, enemyCount);
                        total += replayDamage * replayHits;

                        replay.Add(new MagicWineFoxSpellPreviewStep(
                            cast[rewind].Card, MagicWineFoxSpellPreviewKind.Replay, replayDamage, replayHits));

                        replayState = CreateReplayState(modifiers);
                    }

                    if (replay.Count > 0)
                        segments.Add(new MagicWineFoxSpellPreviewSegment(
                            MagicWineFoxSpellPreviewSegmentKind.Replay, replay));

                    var kept = new List<MagicWineFoxSpellPreviewStep>();

                    for (var rest = index + 1; rest < spells.Count; rest++)
                    {
                        kept.Add(new MagicWineFoxSpellPreviewStep(
                            spells[rest].Card, MagicWineFoxSpellPreviewKind.Kept, 0m, 0));
                    }

                    if (kept.Count > 0)
                        segments.Add(new MagicWineFoxSpellPreviewSegment(
                            MagicWineFoxSpellPreviewSegmentKind.Kept, kept));

                    return new MagicWineFoxSpellReleasePreview(segments, total);
                }

                if (snapshot.Card is IMagicWineFoxSpellModifierCard modifierEntry)
                {
                    if (snapshot.Card is not IMagicWineFoxSpellLookBehindModifierCard and
                        not IMagicWineFoxSpellWandModifierCard)
                    {
                        modifierEntry.ApplyModifier(modifiers);
                        pending.Add(snapshot.Card);
                    }

                    forward.Add(new MagicWineFoxSpellPreviewStep(
                        snapshot.Card, MagicWineFoxSpellPreviewKind.Modifier, 0m, 0));

                    continue;
                }

                if (snapshot.Card is not IMagicWineFoxSpellCard)
                {
                    modifiers.Reset();
                    continue;
                }

                if (lookBehind.TryGetValue(index, out var attached))
                    foreach (var extra in attached)
                    {
                        modifiers.SetCurrentX(extra.XValue);
                        (extra.Card as IMagicWineFoxSpellModifierCard)?.ApplyModifier(modifiers);
                    }

                var (damage, hits) = EstimateDamage(snapshot.Card, modifiers, enemyCount);
                total += damage * hits;

                forward.Add(new MagicWineFoxSpellPreviewStep(
                    snapshot.Card, MagicWineFoxSpellPreviewKind.Cast, damage, hits));

                pending.Clear();
                cast.Add(snapshot);
                used += modifiers.BudgetCost;
                modifiers.Reset();
            }

            if (forward.Count > 0)
                segments.Add(new MagicWineFoxSpellPreviewSegment(
                    MagicWineFoxSpellPreviewSegmentKind.Forward, forward));

            return new MagicWineFoxSpellReleasePreview(segments, total);
        }

        /// <summary>
        ///     把法杖内每张卡的显示数值刷成**计入法术修正符**后的结果。
        /// </summary>
        public static void RefreshWandCardValues(MagicWineFoxSpellSlotPower? power)
        {
            if (power == null)
                return;

            var preview = PreviewRelease(power);
            var adjusted = new HashSet<CardModel>();

            foreach (var segment in preview.Segments)
            {
                foreach (var step in segment.Steps)
                {
                    if (step.Card is not MagicWineFoxSpellCard spell)
                        continue;
                    
                    if (step.Hits > 0 && adjusted.Add(step.Card))
                        spell.ApplyPreviewDamage(step.DamagePerHit);
                }
            }
            
            foreach (var slot in power.Slots)
            {
                if (slot?.Card is MagicWineFoxSpellCard spell && !adjusted.Contains(slot.Card))
                    spell.ClearPreviewDamage();
            }
        }

        private static (decimal Damage, int Hits) EstimateDamage(
            CardModel card,
            MagicWineFoxSpellModifierState state,
            int enemyCount)
        {
            var baseDamage = card is MagicWineFoxSpellCard spell ? spell.PreviewDamage : 0m;
            if (baseDamage <= 0m)
                return (0m, 0);

            var perHit = Math.Max(
                0m,
                Math.Round(
                    (baseDamage + state.DamageBonus) * state.DamageMultiplier,
                    0,
                    MidpointRounding.AwayFromZero));

            var hits = state.CastCount * (1 + state.ExtraDamageStrikes);
            
            if (state.TargetsAllEnemies)
                hits *= enemyCount;

            return (perHit, hits);
        }

        private static async Task CastSpellEntry(
            PlayerChoiceContext choiceContext,
            Player owner,
            CardModel? sourceCard,
            Creature? fallbackTarget,
            MagicWineFoxSpellSlotSnapshot snapshot,
            MagicWineFoxSpellModifierState state)
        {
            if (snapshot.Card is not IMagicWineFoxSpellCard spellCard)
                return;

            var target = ResolveTarget(snapshot.Target, fallbackTarget);

            if (target == null && owner?.Creature?.CombatState is { } targetState)
                target = targetState.HittableEnemies.FirstOrDefault();

            var context = new MagicWineFoxSpellCastContext(
                choiceContext,
                owner,
                target,
                snapshot.Card,
                sourceCard,
                state);

            var casts = state.CastCount;

            for (var i = 0; i < casts; i++)
            {
                var castContext = context;

                if (context.RandomTargets && owner?.Creature?.CombatState is { } randomState)
                {
                    castContext = new MagicWineFoxSpellCastContext(
                        choiceContext,
                        owner,
                        PickRandomEnemy(randomState),
                        snapshot.Card,
                        sourceCard,
                        state);
                }

                var attacked = ResolveAttackedEnemies(castContext).ToList();

                await spellCard.CastAsSpell(castContext);

                await Powers.EternalMelodyPower.ApplyToSpellTargets(
                    choiceContext,
                    owner,
                    snapshot.Card,
                    attacked);
                
                if (state.SummonOnKill > 0m && attacked.Any(enemy => !enemy.IsAlive))
                {
                    await OstyCmd.Summon(
                        choiceContext,
                        owner,
                        state.SummonOnKill,
                        snapshot.Card);
                }

                // 【死灵召唤】：授予「奥斯提横扫」能力——它每回合结束时对全体敌人造成伤害。
                if (state.OstySweepAmount > 0m && owner?.Creature is { } ownerCreature)
                {
                    await PowerCmd.Apply<Powers.NecromanticSummoningPower>(
                        choiceContext,
                        ownerCreature,
                        state.OstySweepAmount,
                        ownerCreature,
                        snapshot.Card);
                }

                if (state.DrawCount > 0m)
                    await CardPileCmd.Draw(choiceContext, state.DrawCount, owner);
            }
        }

        /// <summary>
        ///     倒序重放用的累积器：只继承整轮保留的字段，一次性修正一律不带。
        ///     <para>
        ///         修正符是「打完一张法术就消费掉」的，所以重放时若复用当次快照，
        ///         等于让已被消费的修正再生效一次（双重释放会多放一遍、四重散射会多打一套）。
        ///     </para>
        /// </summary>
        private static MagicWineFoxSpellModifierState CreateReplayState(MagicWineFoxSpellModifierState source)
        {
            var state = new MagicWineFoxSpellModifierState();
            state.SetWandModifierCount(source.WandModifierCount);

            if (source.DrawCount > 0m)
                state.MarkDrawPerSpell(source.DrawCount, source.DrawForModifierEntries);

            return state;
        }

        private static Dictionary<int, List<MagicWineFoxSpellSlotSnapshot>> CollectLookBehindModifiers(
            IReadOnlyList<MagicWineFoxSpellSlotSnapshot> spells)
        {
            var result = new Dictionary<int, List<MagicWineFoxSpellSlotSnapshot>>();

            for (var i = 0; i < spells.Count; i++)
            {
                if (spells[i].Card is not IMagicWineFoxSpellLookBehindModifierCard)
                    continue;

                for (var j = i - 1; j >= 0; j--)
                {
                    if (spells[j].Card is not IMagicWineFoxSpellCard)
                        continue;

                    if (!result.TryGetValue(j, out var list))
                        result[j] = list = [];

                    list.Add(spells[i]);
                    break;
                }
            }

            return result;
        }

        private static Creature? PickRandomEnemy(ICombatState combatState)
        {
            var enemies = combatState.HittableEnemies;
            if (enemies.Count == 0)
                return null;

            return combatState.RunState.Rng.CombatTargets.NextItem(enemies);
        }

        private static IEnumerable<Creature> ResolveAttackedEnemies(MagicWineFoxSpellCastContext context)
        {
            if (context.TargetsAllEnemies)
                return context.Owner?.Creature?.CombatState?.HittableEnemies ?? [];

            return context.Target is { IsAlive: true } target ? [target] : [];
        }

        private static Creature? ResolveTarget(Creature? stored, Creature? fallback)
        {
            if (stored?.IsAlive == true) return stored;
            if (fallback?.IsAlive == true) return fallback;
            return null;
        }
    }
}
