using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Cards.Rare;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     法杖槽位容器。
    /// <para>
    /// 这是法术体系唯一的承载 Power：它内部维护一个按装填顺序排列的槽位列表，
    /// 并持有释放轮数与本回合首次装填是否已用掉两个标量。
    ///         结算顺序来自这个列表，与 Power 的获得顺序无关。
    /// </para>
    /// <para>
    ///         <c>Amount</c> = 槽位容量；<c>DisplayAmount</c> = 当前已装填数量，
    /// 因此 Power 图标上会显示已装填的进度感。
    /// </para>
    /// </summary>
    [RegisterPower]
    public class MagicWineFoxSpellSlotPower : WineFoxPower
    {
        public const int HardSlotCap = 12;

        private List<MagicWineFoxSpellSlotSnapshot?> _slots = [];

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.EnergyIconCake);

        /// <summary>HUD 上显示已装填数量（槽位容量由 <see cref="PowerModel.Amount" /> 表示）。</summary>
        public override int DisplayAmount => LoadedCount;
        
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

        /// <summary>本回合首次装填费用减 1是否已被使用。</summary>
        public bool FirstLoadDiscountUsed
        {
            get => DynamicVars["FirstLoadDiscountUsed"].BaseValue > 0m;
            private set
            {
                DynamicVars["FirstLoadDiscountUsed"].BaseValue = value ? 1m : 0m;
                InvokeDisplayAmountChanged();
            }
        }

        /// <summary>是否已执行过唯一升级。</summary>
        public bool Upgraded { get; private set; }

        /// <summary>永久槽位容量，不受临时偏移影响。</summary>
        public int BaseSlotCapacity => Math.Max(0, (int)Amount);

        /// <summary>在场能力提供的临时偏移合计。</summary>
        private int SlotCapacityOffset =>
            Owner == null
                ? 0
                : Owner.Powers.OfType<ISpellSlotOffsetProvider>().Sum(provider => provider.SlotCapacityOffset);

        public int SlotCapacity =>
            Math.Max(0, Math.Min(BaseSlotCapacity + SlotCapacityOffset, HardSlotCap));

        /// <summary>永久增减槽位容量</summary>
        public void ChangeBaseCapacity(int delta)
        {
            EnsureCapacity(BaseSlotCapacity + delta);
        }

        /// <summary>临时偏移变化后同步槽位与预览。</summary>
        public void RefreshSlotCapacity()
        {
            SyncSlotCount();
            SyncLoadedVar();
            InvalidateSpellPreview();
        }

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

        /// <summary>
        ///     法术链预览的缓存版本号。槽位/修正符/施放次数变化时递增。
        /// </summary>
        public int SpellPreviewVersion { get; private set; }

        /// <summary>缓存住的预览结果，与 <see cref="SpellPreviewVersion" /> 配套。</summary>
        public MagicWineFoxSpellReleasePreview? CachedSpellPreview { get; private set; }

        /// <summary>标记法术链预览缓存失效。</summary>
        public void InvalidateSpellPreview()
        {
            SpellPreviewVersion++;
            CachedSpellPreview = null;
        }

        /// <summary>写入缓存结果（仅当版本号仍然匹配时有效）。</summary>
        public void CacheSpellPreview(int version, MagicWineFoxSpellReleasePreview preview)
        {
            if (version != SpellPreviewVersion)
                return;

            CachedSpellPreview = preview;
        }

        public IReadOnlyList<MagicWineFoxSpellSlotSnapshot?> Slots
        {
            get
            {
                SyncSlotCount();
                return _slots;
            }
        }

        /// <summary>装填一张法术；槽满返回 false。</summary>
        public bool TryLoad(MagicWineFoxSpellSlotSnapshot snapshot)
        {
            SyncSlotCount();

            for (var i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] != null) continue;

                _slots[i] = snapshot;
                SyncLoadedVar();
                InvalidateSpellPreview();
                return true;
            }

            return false;
        }

        /// <summary>取走最旧的一张已装填法术。</summary>
        public MagicWineFoxSpellSlotSnapshot? DrainOldest()
        {
            SyncSlotCount();

            for (var i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                if (slot == null) continue;

                _slots[i] = null;
                SyncLoadedVar();
                InvalidateSpellPreview();
                return slot;
            }

            return null;
        }

        /// <summary>清空全部装填。</summary>
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
            InvalidateSpellPreview();
        }

        public void EnsureCapacity(int capacity)
        {
            capacity = Math.Clamp(capacity, 0, HardSlotCap);
            if (Amount != capacity)
                SetAmount(capacity);

            SyncSlotCount();
            SyncLoadedVar();
            InvalidateSpellPreview();
        }

        public void SetCastCount(int castCount)
        {
            CastCount = Math.Clamp(castCount, 1, 4);
            InvokeDisplayAmountChanged();
            InvalidateSpellPreview();
        }

        /// <summary>
        /// 本回合首次装填费用减 1是否可用。非消耗式查询——是否消费由
        ///     <see cref="ConsumeFirstLoadDiscount" /> 决定。
        /// </summary>
        public bool CanDiscountFirstLoad => !FirstLoadDiscountUsed;

        public void ConsumeFirstLoadDiscount()
        {
            if (FirstLoadDiscountUsed) return;
            FirstLoadDiscountUsed = true;
        }
        
        public override bool TryModifyEnergyCostInCombat(
            CardModel card,
            decimal originalCost,
            out decimal modifiedCost)
        {
            modifiedCost = originalCost;
            
            if (!MagicWineFoxSpellCmd.HasWandRelic(Owner.Player))
                return false;

            if (!CanDiscountFirstLoad)
                return false;
            
            if (card is not MagicWineFoxSpellCard { IsLoadable: true })
                return false;

            if (card.Owner?.Creature != Owner)
                return false;

            var pile = card.Pile?.Type;
            if (pile != PileType.Hand && pile != PileType.Play)
                return false;

            var discounted = originalCost - STS2_WineFox.Commands.MagicWineFoxSpellCmd.FirstLoadDiscount;
            if (discounted >= originalCost)
                return false;

            modifiedCost = Math.Max(0m, discounted);
            return true;
        }

        public bool TryUpgrade(bool expandSlots)
        {
            if (Upgraded) return false;

            Upgraded = true;
            if (expandSlots)
                ChangeBaseCapacity(1);
            else
                SetCastCount(CastCount + 1);

            return true;
        }
        
        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            _slots = [];
            return Task.CompletedTask;
        }

        /// <summary>回合开始时重置首次装填费用减 1，并结算上回合登记的法术修正选择。</summary>
        protected override async Task OnAfterPlayerTurnStart(
            MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext,
            MegaCrit.Sts2.Core.Entities.Players.Player player)
        {
            FirstLoadDiscountUsed = false;
            SpellsCastThisTurn = 0;

            if (_pendingModifiers.Count > 0 && player.Creature == Owner && Owner.Player is { } owner)
            {
                var pending = _pendingModifiers.ToList();
                _pendingModifiers.Clear();

                foreach (var request in pending)
                    await request.Resolve(choiceContext, owner);
            }
        }

        private readonly List<PendingSpellModifierRequest> _pendingModifiers = [];

        /// <summary>登记一个延迟到下回合开始的法术修正选择。</summary>
        public void QueueSpellModifier(PendingSpellModifierRequest request)
        {
            _pendingModifiers.Add(request);
        }

        public bool HasPendingSpellModifiers => _pendingModifiers.Count > 0;

        public int SpellsCastThisTurn { get; private set; }

        /// <summary>本次法杖释放包含的法术总数，供条件类卡牌在释放期间读取。</summary>
        public int ReleaseSpellCount { get; set; }

        public void IncrementSpellsCast()
        {
            SpellsCastThisTurn++;
        }

        public bool ManaRefluxPending { get; set; }

        public int ManaRefluxBonus { get; set; }

        /// <summary>
        /// 回合结束释放：触发条件是拥有本能力而不是持有起始遗物，
        ///     因此跨角色装填法术时也会正常释放。
        /// <para>
        ///         时机与原遗物触发一致（<c>AfterSideTurnEnd</c>，己方回合结束时，默认释放 1 轮）。
        ///         遗物侧的那个触发已移除，避免同一回合释放两次。
        /// </para>
        /// </summary>
        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (side != Owner.Side)
                return;

            // 能力的 Owner 是 Creature，这里用 .Player 取玩家（遗物的 Owner 直接就是 Player）。
            if (Owner.Player is not { } player)
                return;

            await MagicWineFoxSpellCmd.CastAll(choiceContext, player, null, null, isTurnEnd: true);
        }

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
