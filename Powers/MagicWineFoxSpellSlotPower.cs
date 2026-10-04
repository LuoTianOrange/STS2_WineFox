using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     法杖槽位容器。
    ///     <para>
    ///         这是法术体系**唯一**的承载 Power：它内部维护一个按装填顺序排列的槽位列表，
    ///         并持有「释放轮数」与「本回合首次装填是否已用掉」两个标量。
    ///         结算顺序来自这个列表，与 Power 的获得顺序无关。
    ///     </para>
    ///     <para>
    ///         <c>Amount</c> = 槽位容量；<c>DisplayAmount</c> = 当前已装填数量，
    ///         因此 Power 图标上会显示「已装填」的进度感。
    ///     </para>
    /// </summary>
    [RegisterPower]
    public class MagicWineFoxSpellSlotPower : WineFoxPower
    {
        private const int HardSlotCap = 12;

        private readonly List<MagicWineFoxSpellSlotSnapshot?> _slots = [];

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.EnergyIconCake);

        /// <summary>HUD 上显示已装填数量（槽位容量由 <see cref="PowerModel.Amount" /> 表示）。</summary>
        public override int DisplayAmount => LoadedCount;

        /// <summary>
        ///     不在 HUD 的权力图标里显示——槽位状态改由法杖预览条（<c>NSpellSlotBar</c>）可视化。
        ///     与材料类 Power（<c>MaterialPower</c>）同样的隐藏方式。
        /// </summary>
        protected override bool IsVisibleInternal => false;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("CastCount", 1m),
            new("FirstLoadDiscountUsed", 0m)
        ];

        /// <summary>释放轮数：每回合结束时触发几轮释放。默认 1。</summary>
        public int CastCount
        {
            get => Math.Max(1, (int)DynamicVars["CastCount"].BaseValue);
            private set => DynamicVars["CastCount"].BaseValue = Math.Max(1, value);
        }

        /// <summary>本回合「首次装填费用减 1」是否已被使用。</summary>
        public bool FirstLoadDiscountUsed
        {
            get => DynamicVars["FirstLoadDiscountUsed"].BaseValue > 0m;
            private set
            {
                DynamicVars["FirstLoadDiscountUsed"].BaseValue = value ? 1m : 0m;
                InvokeDisplayAmountChanged();
            }
        }

        /// <summary>是否已执行过唯一升级（扩张/速铸二选一，整局只能升级一次）。</summary>
        public bool Upgraded { get; private set; }

        public int SlotCapacity => Math.Max(0, Math.Min((int)Amount, HardSlotCap));

        public int LoadedCount
        {
            get
            {
                SyncSlotCount();
                return _slots.Count(slot => slot != null);
            }
        }

        /// <summary>本次释放最多能释放几张：槽位容量 × 释放轮数。</summary>
        public int ReleaseBudget => SlotCapacity * CastCount;

        public IReadOnlyList<MagicWineFoxSpellSlotSnapshot?> Slots
        {
            get
            {
                SyncSlotCount();
                return _slots;
            }
        }

        /// <summary>装填一张法术；槽满返回 false（法杖不是避难所，装不下就是装不下）。</summary>
        public bool TryLoad(MagicWineFoxSpellSlotSnapshot snapshot)
        {
            SyncSlotCount();

            for (var i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] != null) continue;

                _slots[i] = snapshot;
                SyncLoadedVar();
                return true;
            }

            return false;
        }

        /// <summary>取走最旧的一张已装填法术（从序列头部，保持装填顺序语义）。</summary>
        public MagicWineFoxSpellSlotSnapshot? DrainOldest()
        {
            SyncSlotCount();

            for (var i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                if (slot == null) continue;

                _slots[i] = null;
                SyncLoadedVar();
                return slot;
            }

            return null;
        }

        /// <summary>清空全部装填（返回被清掉的顺序列表，供结算使用）。</summary>
        public List<MagicWineFoxSpellSlotSnapshot> DrainLoadedSpells()
        {
            SyncSlotCount();

            var spells = _slots
                .Where(slot => slot != null)
                .Cast<MagicWineFoxSpellSlotSnapshot>()
                .ToList();

            ClearSlots();
            return spells;
        }

        public void ClearSlots()
        {
            SyncSlotCount();

            for (var i = 0; i < _slots.Count; i++)
                _slots[i] = null;

            SyncLoadedVar();
        }

        public void EnsureCapacity(int capacity)
        {
            capacity = Math.Clamp(capacity, 0, HardSlotCap);
            if (Amount != capacity)
                SetAmount(capacity);

            SyncSlotCount();
            SyncLoadedVar();
        }

        public void SetCastCount(int castCount)
        {
            CastCount = Math.Clamp(castCount, 1, 4);
            InvokeDisplayAmountChanged();
        }

        /// <summary>
        ///     本回合「首次装填费用减 1」是否可用。非消耗式查询——是否消费由
        ///     <see cref="ConsumeFirstLoadDiscount" /> 决定。
        /// </summary>
        public bool CanDiscountFirstLoad => !FirstLoadDiscountUsed;

        public void ConsumeFirstLoadDiscount()
        {
            if (FirstLoadDiscountUsed) return;
            FirstLoadDiscountUsed = true;
        }

        /// <summary>
        ///     起始遗物【狐火杖】：本回合第一次装填的费用减 1（最低 0）。
        ///     <para>
        ///         用游戏原生的费用钩子改写（与 <c>SnowBallOverwhelmingPower</c> 同一套写法），
        ///         因此卡面会直接显示减费后的数值，且能量刚好够时也能打出——
        ///         而不是「先扣后还」。消费时机在 <c>MagicWineFoxSpellCmd.Load</c>。
        ///     </para>
        /// </summary>
        public override bool TryModifyEnergyCostInCombat(
            CardModel card,
            decimal originalCost,
            out decimal modifiedCost)
        {
            modifiedCost = originalCost;

            if (!CanDiscountFirstLoad)
                return false;

            // 只影响可装填的狐火法术。
            if (card is not MagicWineFoxSpellCard { IsLoadable: true })
                return false;

            if (card.Owner?.Creature != Owner)
                return false;

            var pile = card.Pile?.Type;
            if (pile != PileType.Hand && pile != PileType.Play)
                return false;

            // 费用减 FirstLoadDiscount，不低于 0（0 费牌不受影响）。
            var discounted = originalCost - STS2_WineFox.Commands.MagicWineFoxSpellCmd.FirstLoadDiscount;
            if (discounted >= originalCost)
                return false;

            modifiedCost = Math.Max(0m, discounted);
            return true;
        }

        /// <summary>
        ///     唯一升级：<paramref name="expandSlots" /> 为 true 走「扩张」（槽位 +1），
        ///     否则走「速铸」（释放轮数 +1）。已升级过则返回 false。
        /// </summary>
        public bool TryUpgrade(bool expandSlots)
        {
            if (Upgraded) return false;

            Upgraded = true;
            if (expandSlots)
                EnsureCapacity(SlotCapacity + 1);
            else
                SetCastCount(CastCount + 1);

            return true;
        }

        /// <summary>
        ///     Power 被施加时清空槽位——新战斗 / 读档恢复都会走到这里。
        ///     <para>
        ///         槽位内容属于「本场战斗内的临时状态」，绝不能被继承：否则开局法杖里
        ///         会带着上一次已装填的法术，而且它们在释放时**真的会结算并造成伤害**。
        ///     </para>
        /// </summary>
        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            ClearSlots();
            return Task.CompletedTask;
        }

        /// <summary>回合开始时重置「首次装填费用减 1」。</summary>
        protected override Task OnAfterPlayerTurnStart(
            MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext,
            MegaCrit.Sts2.Core.Entities.Players.Player player)
        {
            FirstLoadDiscountUsed = false;
            return Task.CompletedTask;
        }

        // 释放触发点在遗物 MagicWineFoxWand.AfterSideTurnEnd：
        // Power 的 Owner 是 Creature，拿不到 PlayerCombatState；遗物侧的 player 参数可以直接用。
        // 本 Power 只做「槽位容器 + 装填 / 清空」。

        private void SyncSlotCount()
        {
            var capacity = SlotCapacity;
            while (_slots.Count < capacity)
                _slots.Add(null);

            if (_slots.Count > capacity)
                _slots.RemoveRange(capacity, _slots.Count - capacity);
        }

        private void SyncLoadedVar()
        {
            InvokeDisplayAmountChanged();
        }
    }
}
