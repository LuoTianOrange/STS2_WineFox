using MegaCrit.Sts2.Core.Commands;
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
        /// <summary>默认槽位容量（狐火杖初始 3 槽）。</summary>
        public const int DefaultCapacity = 3;

        /// <summary>默认释放轮数。</summary>
        public const int DefaultCastCount = 1;

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
            if (power.TryLoad(snapshot)) return true;

            // 槽满：按设计「装不下就是装不下」，不做自动过载（充能球式的槽满转化不适用于法杖）。
            return false;
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
            if (spells.Count == 0) return;

            await ResolveSpellChain(choiceContext, owner, fallbackTarget, sourceCard, spells, power.ReleaseBudget);
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

            await ResolveSpellChain(choiceContext, owner, fallbackTarget, sourceCard, [oldest], 1);
        }

        public static void ClearLoaded(Player owner)
        {
            GetPower(owner)?.ClearSlots();
        }

        /// <summary>
        ///     顺序释放：累积修正 → 遇法术按次数施放 → 清空累积器。
        ///     这是「修正符只作用于序列中紧随其后的那一张法术」的实现点。
        /// </summary>
        private static async Task ResolveSpellChain(
            PlayerChoiceContext choiceContext,
            Player owner,
            Creature? fallbackTarget,
            CardModel? sourceCard,
            IReadOnlyList<MagicWineFoxSpellSlotSnapshot> spells,
            int releaseBudget)
        {
            var modifiers = new MagicWineFoxSpellModifierState();
            var used = 0;

            foreach (var snapshot in spells)
            {
                if (used >= releaseBudget)
                    break;

                if (snapshot.Card is IMagicWineFoxSpellModifierCard modifier)
                {
                    modifier.ApplyModifier(modifiers);
                    continue;
                }

                if (snapshot.Card is not IMagicWineFoxSpellCard spellCard)
                {
                    // 非法术（状态牌等）：吃掉累积器，防止修正泄漏到后面的法术。
                    modifiers.Reset();
                    continue;
                }

                var target = ResolveTarget(snapshot.Target, fallbackTarget);
                var context = new MagicWineFoxSpellCastContext(
                    choiceContext,
                    owner,
                    target,
                    snapshot.Card,
                    sourceCard,
                    modifiers);

                var casts = modifiers.CastCount;
                for (var i = 0; i < casts; i++)
                    await spellCard.CastAsSpell(context);

                used += modifiers.BudgetCost;
                modifiers.Reset();
            }
        }

        private static Creature? ResolveTarget(Creature? stored, Creature? fallback)
        {
            if (stored?.IsAlive == true) return stored;
            if (fallback?.IsAlive == true) return fallback;
            return null;
        }
    }
}
