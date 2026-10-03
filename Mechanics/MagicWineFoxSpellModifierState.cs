namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     一次释放过程的修正符累积器。
    ///     <para>
    ///         这**不是** Power——它是 <c>MagicWineFoxSpellCmd</c> 在一次释放流程里 new 出来的局部变量。
    ///         结算顺序由「装填顺序」（<c>MagicWineFoxSpellSlotPower</c> 内部的槽位列表）决定，与 Power 的获得顺序无关。
    ///     </para>
    ///     <para>
    ///         消费规则：遍历槽位时，遇到修正符就累积；遇到法术就按 <see cref="CastCount" /> 施放，然后
    ///         <see cref="Reset" />。因此修正符只作用于「序列中紧随其后的那一张法术」。
    ///     </para>
    /// </summary>
    public class MagicWineFoxSpellModifierState
    {
        /// <summary>附加伤害（加法叠加）。交给施放方自行决定是加到伤害上还是别的用途。</summary>
        public decimal DamageBonus { get; private set; }

        /// <summary>额外释放轮数（不叠加标记，只加次数）。</summary>
        public int ExtraCasts { get; private set; }

        /// <summary>
        ///     本段额外释放是否**不消耗**本轮释放的施法名额。
        ///     对应【双重释放符】的「额外释放且不占名额」。
        /// </summary>
        public bool ExtraCastsAreBudgetFree { get; private set; }

        /// <summary>本次施放实际执行几次：基础 1 次 + 额外次数。</summary>
        public int CastCount => 1 + Math.Max(0, ExtraCasts);

        /// <summary>本次施放是否占用施法名额（不占则只记 1 个名额）。</summary>
        public int BudgetCost => ExtraCastsAreBudgetFree ? 1 : CastCount;

        /// <summary>累积器当前是否有任何修正（用于修正符空放时的判定与调试）。</summary>
        public bool IsEmpty => DamageBonus == 0m && ExtraCasts == 0 && !ExtraCastsAreBudgetFree;

        public void AddDamageBonus(decimal amount)
        {
            DamageBonus += amount;
        }

        public void AddExtraCasts(int amount)
        {
            ExtraCasts += amount;
        }

        public void MarkExtraCastsBudgetFree()
        {
            ExtraCastsAreBudgetFree = true;
        }

        /// <summary>消费掉当前累积的修正。每施放一张法术后必须调用，否则修正会泄漏到后续法术。</summary>
        public void Reset()
        {
            DamageBonus = 0m;
            ExtraCasts = 0;
            ExtraCastsAreBudgetFree = false;
        }
    }
}
