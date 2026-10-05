using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Commands
{
    /// <summary>
    ///     法杖/法术系统的唯一入口命令。
    ///     <para>
    ///         结算模型（对应设计文档 v0.2.6）：
    ///         装填 = 支付卡面费用后把法术放进槽位；释放 = 回合结束按槽位顺序释放，
    ///         修正符累积、遇到法术后消费并清空。
    ///     </para>
    ///     <para>
    ///         注意：法术体系**刻意不复用** <c>Combat/Magic</c> 的咏唱管线
    ///         （<c>MagicDamage</c> / <c>ChantPower</c>），也不再引用 <c>Cards/Deleted/Magic</c> 下的任何卡。
    ///     </para>
    /// </summary>
    public static class MagicWineFoxSpellCmd
    {
        /// <summary>默认槽位容量（狐火杖初始 4 槽；「扩张」升级后为 5）。</summary>
        public const int DefaultCapacity = 4;

        /// <summary>默认释放轮数。</summary>
        public const int DefaultCastCount = 1;

        /// <summary>起始遗物【狐火杖】：每回合第一次装填减免的费用。</summary>
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
            bool isModifier = false)
        {
            var owner = card.Owner;
            var power = await EnsurePower(owner);
            if (power == null) return false;

            var snapshot = new MagicWineFoxSpellSlotSnapshot(card.CreateClone(), play.Target, isModifier);

            // 序列回响：把这张法术记为「始终释放」的法术（照常装填，不拦截）。
            var echo = owner?.Creature?.Powers.OfType<Powers.EchoesSequencePower>().FirstOrDefault();
            echo?.TryBind(card);

            // 槽满：按设计「装不下就是装不下」，不做自动过载（充能球式的槽满转化不适用于法杖）。
            if (!power.TryLoad(snapshot)) return false;

            // 起始遗物【狐火杖】：每回合第一次装填的费用减 1（最低 0）。
            // 减费由 MagicWineFoxSpellSlotPower.TryModifyEnergyCostInCombat 直接改写费用，
            // 这里只负责「消费」这次减费，使其每回合仅生效一次。
            if (power.CanDiscountFirstLoad)
                power.ConsumeFirstLoadDiscount();

            return true;
        }

        /// <summary>释放：按装填顺序释放全部已装填法术（受释放轮数约束的施法名额）。</summary>
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

            // 逆遍历等修正符可能把「其后方的法术」原样退回法杖；
            // 此时法杖已清空，因此它们会落在最前面的槽位。
            foreach (var snapshot in preserved)
                if (!power.TryLoad(snapshot))
                    break;

            // 序列回响：法杖清空后自动把绑定的回响法术装回第一个槽位，
            // 于是它永远排在序列最前——也就吃不到任何修正符加成。
            EnsureEchoLoaded(owner);
        }

        /// <summary>把【序列回响】绑定的法术自动装回法杖（法杖已被清空，因此它落在第一个槽位）。</summary>
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
        ///     这是「修正符只作用于序列中紧随其后的那一张法术」的实现点。
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

            foreach (var snapshot in spells)
            {
                if (snapshot.Card is IMagicWineFoxSpellWandModifierCard wandModifier)
                    wandModifier.ApplyModifier(modifiers);
            }

            for (var index = 0; index < spells.Count; index++)
            {
                var snapshot = spells[index];

                if (used >= releaseBudget)
                    break;

                if (snapshot.Card is IMagicWineFoxSpellReverseModifierCard)
                {
                    Main.Logger.Info($"[SpellReverse] 触发逆遍历，倒序重放 {history.Count} 条历史");

                    for (var rewind = history.Count - 1; rewind >= 0; rewind--)
                    {
                        await CastSpellEntry(
                            choiceContext,
                            owner,
                            sourceCard,
                            fallbackTarget,
                            history[rewind],
                            CreateReplayState(modifiers));
                    }

                    return spells.Skip(index + 1).ToList();
                }

                if (snapshot.Card is IMagicWineFoxSpellModifierCard modifierEntry)
                {
                    if (snapshot.Card is not IMagicWineFoxSpellLookBehindModifierCard and
                        not IMagicWineFoxSpellWandModifierCard)
                        modifierEntry.ApplyModifier(modifiers);

                    if (modifiers.DrawForModifierEntries && modifiers.DrawCount > 0m)
                        await CardPileCmd.Draw(choiceContext, modifiers.DrawCount, owner);

                    continue;
                }

                if (snapshot.Card is not IMagicWineFoxSpellCard)
                {
                    // 非法术（状态牌等）：吃掉累积器，防止修正泄漏到后面的法术。
                    modifiers.Reset();
                    continue;
                }

                if (lookBehind.TryGetValue(index, out var attached))
                    foreach (var extra in attached)
                        extra.ApplyModifier(modifiers);

                history.Add(snapshot);

                await CastSpellEntry(choiceContext, owner, sourceCard, fallbackTarget, snapshot, modifiers);

                used += modifiers.BudgetCost;
                modifiers.Reset();
            }

            return [];
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

            Main.Logger.Info(
                $"[SpellCast] {snapshot.Card.Id.Entry} casts={casts} " +
                $"strikes={state.ExtraDamageStrikes} mult={state.DamageMultiplier} bonus={state.DamageBonus} " +
                $"allEnemies={state.TargetsAllEnemies} random={state.RandomTargets}");
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

                if (state.DrawCount > 0m)
                    await CardPileCmd.Draw(choiceContext, state.DrawCount, owner);
            }
        }

        /// <summary>
        ///     倒序重放用的累积器：**只继承整轮保留的字段**，一次性修正一律不带。
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

        private static Dictionary<int, List<IMagicWineFoxSpellModifierCard>> CollectLookBehindModifiers(
            IReadOnlyList<MagicWineFoxSpellSlotSnapshot> spells)
        {
            var result = new Dictionary<int, List<IMagicWineFoxSpellModifierCard>>();

            for (var i = 0; i < spells.Count; i++)
            {
                if (spells[i].Card is not IMagicWineFoxSpellLookBehindModifierCard lookBehind)
                    continue;

                for (var j = i - 1; j >= 0; j--)
                {
                    if (spells[j].Card is not IMagicWineFoxSpellCard)
                        continue;

                    if (!result.TryGetValue(j, out var list))
                        result[j] = list = [];

                    list.Add(lookBehind);
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
