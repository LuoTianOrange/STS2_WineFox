using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Combat.Magic;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2_WineFox.Relics;

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

        /// <summary>
        ///     其他角色的默认槽位数。
        /// </summary>
        public const int NonWandDefaultCapacity = 2;

        /// <summary>起始遗物：每回合第一次装填减免的费用。</summary>
        public const int FirstLoadDiscount = 1;

        public static bool HasWandRelic(Player? player)
        {
            return player?.Relics.OfType<MagicWineFoxWand>().Any() == true;
        }

        public static async Task<bool> GiveDoubleReleaseSigil(Player owner)
        {
            if (owner?.Creature?.CombatState is not { } combatState)
                return false;

            var sigil = combatState.CreateCard<DoubleReleaseSigil>(owner);
            var instance = await CardPileCmd.AddGeneratedCardToCombat(sigil, PileType.Hand, owner);
            CardCmd.PreviewCardPileAdd(instance);
            return true;
        }

        public static async Task<MagicWineFoxSpellSlotPower?> EnsurePower(Player owner,
            PlayerChoiceContext? choiceContext = null, int? capacity = null,
            int castCount = DefaultCastCount)
        {
            if (owner?.Creature == null) return null;

            var requested = Math.Clamp(
                capacity ?? (HasWandRelic(owner) ? DefaultCapacity : NonWandDefaultCapacity),
                0,
                MagicWineFoxSpellSlotPower.HardSlotCap);

            var existing = owner.Creature.Powers.OfType<MagicWineFoxSpellSlotPower>().FirstOrDefault();
            if (existing != null)
            {
                existing.EnsureCapacity(Math.Max(existing.BaseSlotCapacity, requested));

                return existing;
            }

            var applied = await PowerCmd.Apply<MagicWineFoxSpellSlotPower>(
                choiceContext ?? new ThrowingPlayerChoiceContext(),
                owner.Creature,
                requested,
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

        /// <summary>本次法杖释放包含的法术总数（不含修正符）；非释放期间回落到槽内当前数量。</summary>
        public static int CountLoadedSpells(Player? owner)
        {
            if (owner == null)
                return 0;

            var power = GetPower(owner);
            if (power == null)
                return 0;

            if (power.ReleaseSpellCount > 0)
                return power.ReleaseSpellCount;

            return power.Slots.Count(slot =>
                slot?.Card is IMagicWineFoxSpellCard and not IMagicWineFoxSpellModifierCard);
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
            var power = await EnsurePower(owner, choiceContext);
            if (power == null) return false;

            var snapshot = new MagicWineFoxSpellSlotSnapshot(
                card.CreateClone(), play.Target, isModifier, xValue, card);

            // 序列回响：把这张法术记为「始终释放」的法术。
            var echo = owner?.Creature?.Powers.OfType<EchoesSequencePower>().FirstOrDefault();
            echo?.TryBind(card);

            // 槽满：按设计「装不下就是装不下」，不做自动过载（充能球式的槽满转化不适用于法杖）。
            if (!power.TryLoad(snapshot)) return false;


            // 复现：若它打出时法杖为空、正等待认领，则认领这张刚装填的法术。
            owner?.Creature?.Powers.OfType<ReproductionPower>().FirstOrDefault()?.TryBind(snapshot);

            if (power.CanDiscountFirstLoad)
                power.ConsumeFirstLoadDiscount();

            // 奥术电容：装填攻击型法术时累积蓄能。
            var capacitor = owner?.Creature?.Powers.OfType<ArcaneCapacitorPower>().FirstOrDefault();
            if (capacitor != null)
                await capacitor.OnSpellLoaded(choiceContext, card);

            // 装填后立刻刷新法杖内所有卡的显示数值（Power + 法术修正符）。
            RefreshWandCardValues(power);

            var experiment = owner?.Creature?.Powers.OfType<SpellExperimentPower>().FirstOrDefault();
            if (experiment != null)
                await experiment.TryReleaseWhenStaffFull(choiceContext, owner!, power);

            return true;
        }

        /// <summary>释放：按装填顺序释放全部已装填法术。</summary>
        public static async Task CastAll(
            PlayerChoiceContext choiceContext,
            Player owner,
            Creature? fallbackTarget,
            CardModel? sourceCard,
            bool isTurnEnd = false)
        {
            var power = GetPower(owner);
            if (power == null) return;

            var loaded = power.DrainLoadedSpells();

            power.ReleaseSpellCount = loaded.Count(snapshot =>
                snapshot.Card is IMagicWineFoxSpellCard and not IMagicWineFoxSpellModifierCard);

            var carried = new List<MagicWineFoxSpellSlotSnapshot>();
            if (!isTurnEnd)
                carried.AddRange(loaded.Where(snapshot => snapshot.Card is SpellOverload));

            var overloadCount = loaded.Count(snapshot => snapshot.Card is SpellOverload);
            if (overloadCount > 0)
            {
                var overloadState = new MagicWineFoxSpellModifierState();

                foreach (var entry in loaded)
                {
                    if (entry.Card is not SpellOverload overload)
                        continue;

                    overload.OverloadCount = overloadCount;

                    await overload.CastAsSpell(new(
                        choiceContext, owner, null, entry.Card, sourceCard, overloadState));
                }
            }

            var spells = loaded;

            var preserved = spells.Count > 0
                ? await ResolveSpellChain(
                    choiceContext, owner, fallbackTarget, sourceCard, spells,
                    power.ReleaseBudget, power.CastCount)
                : [];

            if (power.ManaRefluxPending)
            {
                power.ManaRefluxPending = false;
                var amount = power.SpellsCastThisTurn + power.ManaRefluxBonus;
                if (amount > 0 && owner.Creature is { } refluxTarget)
                    await PowerCmd.Apply<EnergyNextTurnPower>(
                        choiceContext, refluxTarget, amount, refluxTarget, sourceCard);
            }

            foreach (var snapshot in carried.Concat(preserved))
                if (!power.TryLoad(snapshot))
                    break;

            EnsureEchoLoaded(owner);
            RefreshWandCardValues(power);

            power.ReleaseSpellCount = 0;
        }

        /// <summary>把【序列回响】绑定的法术自动装回法杖。</summary>
        private static void EnsureEchoLoaded(Player owner)
        {
            var echo = owner?.Creature?.Powers.OfType<EchoesSequencePower>().FirstOrDefault();
            if (echo?.EchoSpell is not { } template)
                return;

            var power = GetPower(owner);
            if (power == null)
                return;

            // 每次装回都用新的克隆，避免复用已被结算过的实例。
            power.TryLoad(new(template.CreateClone(), null, false, 0, template));
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

            power.ReleaseSpellCount = 1 + power.Slots.Count(slot =>
                slot?.Card is IMagicWineFoxSpellCard and not IMagicWineFoxSpellModifierCard);

            var preserved = await ResolveSpellChain(
                choiceContext,
                owner,
                fallbackTarget,
                sourceCard,
                [oldest],
                1,
                1);

            foreach (var snapshot in preserved)
                if (!power.TryLoad(snapshot))
                    break;

            power.ReleaseSpellCount = 0;
        }

        public static void ClearLoaded(Player owner)
        {
            GetPower(owner)?.ClearSlots();
        }

        /// <summary>
        ///     把修正符登记的伤害修正抬高本张法术的伤害值，并同步写到实体卡上，使加成随卡留在本场战斗。
        /// </summary>
        private static void ApplyDamageCorrection(
            MagicWineFoxSpellSlotSnapshot snapshot,
            MagicWineFoxSpellModifierState modifiers)
        {
            var amount = modifiers.ConsumeDamageCorrection();

            if (amount <= 0m)
                return;

            RaiseDamage(snapshot.Card, amount);

            if (snapshot.SourceCard is { } source && !ReferenceEquals(source, snapshot.Card))
                RaiseDamage(source, amount);
        }

        private static void RaiseDamage(CardModel card, decimal amount)
        {
            if (!card.DynamicVars.TryGetValue("Damage", out var damage))
                return;

            damage.BaseValue += amount;
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
            int releaseBudget,
            int rounds)
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
                    modifiers.SetCurrentTarget(snapshot.Target);
                    wandModifier.ApplyModifier(modifiers);
                    modifiers.MarkWandModifierApplied();
                }
            }

            for (var round = 0; round < Math.Max(1, rounds) + Math.Max(0, modifiers.ExtraRounds); round++)
            {
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

                    if (snapshot.Card is SpellOverload)
                        continue;

                    if (snapshot.Card is IMagicWineFoxSpellModifierCard modifierEntry)
                    {
                        if (snapshot.Card is not IMagicWineFoxSpellLookBehindModifierCard and
                            not IMagicWineFoxSpellWandModifierCard)
                        {
                            modifiers.SetCurrentX(snapshot.XValue);
                            modifierEntry.ApplyModifier(modifiers);
                            modifiers.MarkModifierApplied();
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
                            modifiers.MarkModifierApplied();
                        }

                    ApplyDamageCorrection(snapshot, modifiers);

                    history.Add(snapshot);

                    await CastSpellEntry(choiceContext, owner, sourceCard, fallbackTarget, snapshot, modifiers);

                    used += modifiers.BudgetCost;
                    modifiers.Reset();
                }

                if (used >= releaseBudget)
                    break;
            }

            return [];
        }

        /// <summary>
        ///     预览本次释放：与 <see cref="ResolveSpellChain" /> 使用同一套规则推演，
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

            var previewSpellCount = spells.Count(snapshot => snapshot.Card is IMagicWineFoxSpellCard
                and not IMagicWineFoxSpellModifierCard);

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
                    modifiers.SetCurrentTarget(snapshot.Target);
                    wandModifier.ApplyModifier(modifiers);
                }
            }

            for (var round = 0;
                 round < Math.Max(1, power.CastCount) && used < power.ReleaseBudget;
                 round++)
            for (var index = 0; index < spells.Count; index++)
            {
                var snapshot = spells[index];

                if (used >= power.ReleaseBudget)
                    break;

                if (snapshot.Card is IMagicWineFoxSpellReverseModifierCard)
                {
                    forward.Add(new(
                        snapshot.Card, MagicWineFoxSpellPreviewKind.Reverse, 0m, 0));
                    segments.Add(new(
                        MagicWineFoxSpellPreviewSegmentKind.Forward, forward));

                    var replay = new List<MagicWineFoxSpellPreviewStep>();

                    for (var back = pending.Count - 1; back >= 0; back--)
                    {
                        replay.Add(new(
                            pending[back], MagicWineFoxSpellPreviewKind.Modifier, 0m, 0));
                    }

                    var replayState = modifiers.Copy();

                    for (var rewind = cast.Count - 1; rewind >= 0; rewind--)
                    {
                        var (replayDamage, replayHits) = EstimateDamage(
                            cast[rewind].Card, replayState, enemyCount, previewSpellCount);
                        total += replayDamage * replayHits;

                        replay.Add(new(
                            cast[rewind].Card, MagicWineFoxSpellPreviewKind.Replay, replayDamage, replayHits));

                        replayState = CreateReplayState(modifiers);
                    }

                    if (replay.Count > 0)
                        segments.Add(new(
                            MagicWineFoxSpellPreviewSegmentKind.Replay, replay));

                    var kept = new List<MagicWineFoxSpellPreviewStep>();

                    for (var rest = index + 1; rest < spells.Count; rest++)
                    {
                        kept.Add(new(
                            spells[rest].Card, MagicWineFoxSpellPreviewKind.Kept, 0m, 0));
                    }

                    if (kept.Count > 0)
                        segments.Add(new(
                            MagicWineFoxSpellPreviewSegmentKind.Kept, kept));

                    return new(segments, total);
                }

                if (snapshot.Card is IMagicWineFoxSpellModifierCard modifierEntry)
                {
                    if (snapshot.Card is not IMagicWineFoxSpellLookBehindModifierCard and
                        not IMagicWineFoxSpellWandModifierCard)
                    {
                        modifierEntry.ApplyModifier(modifiers);
                        pending.Add(snapshot.Card);
                    }

                    forward.Add(new(
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

                var correction = modifiers.ConsumeDamageCorrection();

                if (correction > 0m)
                    modifiers.AddDamageBonus(correction);

                var (damage, hits) = EstimateDamage(snapshot.Card, modifiers, enemyCount, previewSpellCount);
                total += damage * hits;

                forward.Add(new(
                    snapshot.Card, MagicWineFoxSpellPreviewKind.Cast, damage, hits));

                pending.Clear();
                cast.Add(snapshot);
                used += modifiers.BudgetCost;
                modifiers.Reset();
            }

            if (forward.Count > 0)
                segments.Add(new(
                    MagicWineFoxSpellPreviewSegmentKind.Forward, forward));

            return new(segments, total);
        }

        /// <summary>
        ///     把法杖内每张卡的显示数值刷成计入法术修正符后的结果。
        ///     <para>
        ///         结果按 <see cref="MagicWineFoxSpellSlotPower.SpellPreviewVersion" /> 缓存：
        ///         本法术链推演代价不低，而它会被槽位悬停（<c>NSpellSlotBar.AttachHoverTip</c>）
        ///         高频触发，鼠标快速划过槽位时会连续重算导致卡顿。槽位内容不变时直接复用缓存。
        ///     </para>
        /// </summary>
        public static void RefreshWandCardValues(MagicWineFoxSpellSlotPower? power)
        {
            if (power == null)
                return;

            var version = power.SpellPreviewVersion;
            var preview = power.CachedSpellPreview;

            if (preview == null)
            {
                preview = PreviewRelease(power);
                power.CacheSpellPreview(version, preview);
            }

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

            // 格挡类法术：把 IMagicBlockModifier（如【压缩施法】）的加成算进显示。
            // 没有格挡变量的法术（如【狐火弹】）必须跳过——DynamicVars["Block"] 会抛异常。
            foreach (var slot in power.Slots)
            {
                if (slot?.Card is not MagicWineFoxSpellCard spell || !spell.HasBlockVar)
                    continue;

                if (spell.PreviewBlock > 0m)
                    spell.ApplyPreviewBlock(spell.PreviewBlock);
                else
                    spell.ClearPreviewBlock();
            }
        }

        private static (decimal Damage, int Hits) EstimateDamage(
            CardModel card,
            MagicWineFoxSpellModifierState state,
            int enemyCount,
            int spellCount = 0)
        {
            var baseDamage = card is MagicWineFoxSpellCard spell
                ? spell.PreviewDamage + spell.PreviewExtraDamage(spellCount)
                : 0m;
            if (baseDamage <= 0m)
                return (0m, 0);

            var perHit = Math.Max(
                0m,
                Math.Round(
                    (baseDamage + state.DamageBonus) * state.DamageMultiplier,
                    0,
                    MidpointRounding.AwayFromZero));

            // 与 DamageWithModifiers 保持一致：把 IMagicDamageModifier（如【压缩施法】）
            // 的加成也算进来，否则卡面/槽位预览显示不出这部分伤害。
            if (card._owner?.Creature?.CombatState is { } combatState)
            {
                var previewTarget = combatState.HittableEnemies.FirstOrDefault();
                perHit = MagicDamage.Resolve(card, perHit, previewTarget);
            }

            var traits = card is MagicWineFoxSpellCard spellTraits ? spellTraits.Traits : SpellTraits.Default;

            var strikeMultiplier = traits.AcceptsExtraStrikes
                ? 1 + Math.Max(0, state.ExtraDamageStrikes)
                : 1;

            var hits = state.CastCount * Math.Max(0, traits.HitCount) * strikeMultiplier;

            if (state.TargetsAllEnemies || traits.AreaOfEffect)
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

            var target = state.ForcedTarget ?? ResolveTarget(snapshot.Target, fallbackTarget);

            if (owner?.Creature is { } caster && target == caster)
                target = null;

            if (target == null)
            {
                var traits = (snapshot.Card as MagicWineFoxSpellCard)?.Traits ?? SpellTraits.Default;

                if (!traits.TargetSelf)
                    target = owner?.Creature?.CombatState?.HittableEnemies.FirstOrDefault();
            }

            if (owner.Creature.Powers.OfType<SourceOfChaosPower>().Any())
                state.MarkRandomTargets();

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
                    castContext = new(
                        choiceContext,
                        owner,
                        PickRandomEnemy(randomState),
                        snapshot.Card,
                        sourceCard,
                        state);
                }

                var attackCandidates = state.SummonOnKill > 0m
                    ? CaptureAttackCandidates(castContext)
                    : [];

                var trackedEnemies = (state.DamageToBlockRatio > 0m ||
                                      state.DamageToBurnRatio > 0m ||
                                      state.DamageToPoisonRatio > 0m) &&
                                     owner?.Creature?.CombatState is { } trackingState
                    ? trackingState.HittableEnemies.ToDictionary(enemy => enemy, enemy => enemy.CurrentHp)
                    : null;

                var debuffsBefore = state.DebuffAmplify > 1m && owner?.Creature?.CombatState is { } debuffState
                    ? SnapshotDebuffs(debuffState)
                    : null;

                await spellCard.CastAsSpell(castContext);

                if (trackedEnemies != null && owner?.Creature is { } ratioOwner)
                {
                    var dealt = trackedEnemies.Sum(pair => Math.Max(0, pair.Value - pair.Key.CurrentHp));

                    var gained = Math.Floor(dealt * state.DamageToBlockRatio);

                    if (gained > 0m)
                        await CreatureCmd.GainBlock(ratioOwner, gained, ValueProp.Unpowered, null);

                    await ApplyDamageTrailDebuffs(
                        choiceContext,
                        ratioOwner,
                        trackedEnemies,
                        state,
                        snapshot.Card);
                }

                if (debuffsBefore != null && owner?.Creature is { } amplifyOwner)
                    await AmplifyNewDebuffs(
                        choiceContext,
                        amplifyOwner,
                        debuffsBefore,
                        state.DebuffAmplify,
                        snapshot.Card);

                if (state.SecondaryCastRatio > 0m && owner is { } secondaryOwner &&
                    owner.Creature?.CombatState is { } secondaryState)
                {
                    var secondary = PickRandomEnemy(secondaryState, castContext.Target);

                    if (secondary != null)
                    {
                        var secondaryContext = new MagicWineFoxSpellCastContext(
                            choiceContext,
                            secondaryOwner,
                            secondary,
                            snapshot.Card,
                            sourceCard,
                            state)
                        {
                            DamageScale = state.SecondaryCastRatio
                        };

                        await spellCard.CastAsSpell(secondaryContext);
                    }
                }

                if (snapshot.Card is not MagicWineFoxSpellCard { Traits.CountsAsCast: false })
                    GetPower(owner)?.IncrementSpellsCast();

                // 法术余烬：攻击型法术被释放时给所有敌人叠灼烧。
                if (snapshot.Card is MagicWineFoxSpellCard { Traits.AttackSpell: true })
                    await SpellAshesPower.ApplyToAllEnemies(choiceContext, owner, snapshot.Card);

                // 「是否攻击到敌人」卡牌声明判定，
                var attacked = snapshot.Card is MagicWineFoxSpellCard { Traits.AttackSpell: true }
                    ? ResolveAttackedEnemies(castContext).ToList()
                    : [];

                // 永恒旋律只在**真的攻击到敌人**的法术上触发（判据是 Traits.AttackSpell 声明，
                // 与 CardType 无关）：纯防御/自身法术没有 attacked 目标，自然不会削上限。
                if (attacked.Count > 0)
                    await EternalMelodyPower.ApplyToSpellTargets(
                        choiceContext,
                        owner,
                        snapshot.Card,
                        attacked);

                if (state.ShareDamageOnHit && owner?.Creature is { } shareOwner)
                    foreach (var enemy in attacked.Where(enemy => enemy.IsAlive &&
                                                                  !enemy.HasPower<GravitationalSingularityPower>()))
                        await PowerCmd.Apply<GravitationalSingularityPower>(
                            choiceContext,
                            enemy,
                            1m,
                            shareOwner,
                            snapshot.Card);

                if (state.SummonOnKill > 0m)
                {
                    var kills = attackCandidates.Count(enemy => !enemy.IsAlive);

                    for (var kill = 0; kill < kills; kill++)
                    {
                        await OstyCmd.Summon(
                            choiceContext,
                            owner,
                            state.SummonOnKill,
                            snapshot.Card);
                    }
                }

                // 死灵召唤
                if (state.OstySweepAmount > 0m && owner?.Creature is { } ownerCreature)
                {
                    await PowerCmd.Apply<NecromanticSummoningPower>(
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
        ///     记录敌人当前身上的负面效果层数，供「负面效果翻倍」类修正符在法术结算后补差。
        /// </summary>
        private static Dictionary<Creature, Dictionary<PowerModel, int>> SnapshotDebuffs(
            ICombatState combatState)
        {
            var result = new Dictionary<Creature, Dictionary<PowerModel, int>>();

            foreach (var enemy in combatState.HittableEnemies)
                result[enemy] = enemy.Powers
                    .Where(power => power.Type == PowerType.Debuff)
                    .ToDictionary(power => power, power => power.Amount);

            return result;
        }

        /// <summary>
        ///     把法术新施加的负面效果层数按倍率补足差额。
        /// </summary>
        private static async Task AmplifyNewDebuffs(
            PlayerChoiceContext choiceContext,
            Creature applier,
            Dictionary<Creature, Dictionary<PowerModel, int>> before,
            decimal factor,
            CardModel? sourceCard)
        {
            if (factor <= 1m)
                return;

            foreach (var (enemy, snapshot) in before)
            {
                foreach (var power in enemy.Powers.Where(entry => entry.Type == PowerType.Debuff).ToList())
                {
                    var previous = snapshot.TryGetValue(power, out var amount) ? amount : 0m;
                    var delta = power.Amount - previous;

                    if (delta <= 0m)
                        continue;

                    var extra = Math.Floor(delta * (factor - 1m));

                    if (extra <= 0m)
                        continue;

                    await PowerCmd.Apply(
                        choiceContext,
                        power,
                        enemy,
                        extra,
                        applier,
                        sourceCard);
                }
            }
        }

        /// <summary>
        ///     把本次法术造成的伤害按比例转为灼烧/中毒层数。
        /// </summary>
        private static async Task ApplyDamageTrailDebuffs(
            PlayerChoiceContext choiceContext,
            Creature caster,
            Dictionary<Creature, int> tracked,
            MagicWineFoxSpellModifierState state,
            CardModel? sourceCard)
        {
            if (state.DamageToBurnRatio <= 0m && state.DamageToPoisonRatio <= 0m)
                return;

            foreach (var (enemy, hpBefore) in tracked)
            {
                var dealt = Math.Max(0, hpBefore - enemy.CurrentHp);

                if (dealt <= 0)
                    continue;

                if (state.DamageToBurnRatio > 0m)
                {
                    var burn = Math.Floor(dealt * state.DamageToBurnRatio);

                    if (burn > 0m)
                        await PowerCmd.Apply<BurningPower>(
                            choiceContext,
                            enemy,
                            burn,
                            caster,
                            sourceCard);
                }

                if (state.DamageToPoisonRatio > 0m)
                {
                    var poison = Math.Floor(dealt * state.DamageToPoisonRatio);

                    if (poison > 0m)
                        await PowerCmd.Apply<PoisonPower>(
                            choiceContext,
                            enemy,
                            poison,
                            caster,
                            sourceCard);
                }
            }
        }

        /// <summary>
        ///     倒序重放用的累积器：只继承整轮保留的字段，一次性修正一律不带。
        ///     <para>
        ///         修正符是打完一张法术就消费掉的，所以重放时若复用当次快照，
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

        private static Creature? PickRandomEnemy(ICombatState combatState, Creature? exclude = null)
        {
            var enemies = exclude == null
                ? combatState.HittableEnemies
                : combatState.HittableEnemies.Where(enemy => enemy != exclude).ToList();

            if (enemies.Count == 0)
                return null;

            return combatState.RunState.Rng.CombatTargets.NextItem(enemies);
        }

        private static List<Creature> CaptureAttackCandidates(MagicWineFoxSpellCastContext context)
        {
            if (context.SourceCard is not MagicWineFoxSpellCard { Traits.AttackSpell: true })
                return [];

            if (context.TargetsAllEnemies)
                return context.Owner?.Creature?.CombatState?.HittableEnemies.ToList() ?? [];

            return context.Target is { } target ? [target] : [];
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
