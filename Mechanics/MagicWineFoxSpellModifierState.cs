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

        /// <summary>
        ///     伤害倍率（乘法叠加，默认 1）。对应【穿刺魔弹】的「减少 20% 伤害」。
        ///     与其他倍率相乘，因此多张修正符可以叠加。
        /// </summary>
        public decimal DamageMultiplier { get; private set; } = 1m;

        /// <summary>
        ///     下一个法术是否改为**对所有敌人**结算。
        ///     对应【穿刺魔弹】的「下一个法术对所有敌人造成伤害」。
        /// </summary>
        public bool TargetsAllEnemies { get; private set; }

        public bool RandomTargets { get; private set; }

        /// <summary>
        ///     当前法术**额外结算几次伤害**（只重复伤害，不重跑法术的其他效果，如抽牌、上异常）。
        ///     随 <see cref="Reset" /> 消费，因此只作用于紧随其后的那一张法术。
        /// </summary>
        public int ExtraDamageStrikes { get; private set; }

        /// <summary>
        ///     整轮释放持续的抽牌数：本次释放里每打出一张法术就抽这么多张。
        ///     <para>与上面的字段不同，它**不随 <see cref="Reset" /> 清除**——对应「整轮生效」的修正符。</para>
        /// </summary>
        public decimal DrawCount { get; private set; }

        /// <summary>整轮抽牌是否也覆盖**修正符条目**（对应本卡的升级效果）。</summary>
        public bool DrawForModifierEntries { get; private set; }

        /// <summary>
        ///     本次释放时法杖里**装填了多少张法术修正**（整轮固定，不随 <see cref="Reset" /> 清除）。
        /// </summary>
        public int WandModifierCount { get; private set; }

        /// <summary>本次施放实际执行几次：基础 1 次 + 额外次数。</summary>
        public int CastCount => 1 + Math.Max(0, ExtraCasts);

        /// <summary>本次施放是否占用施法名额（不占则只记 1 个名额）。</summary>
        public int BudgetCost => ExtraCastsAreBudgetFree ? 1 : CastCount;

        /// <summary>累积器当前是否有任何修正（用于修正符空放时的判定与调试）。</summary>
        public bool IsEmpty =>
            DamageBonus == 0m && ExtraCasts == 0 && !ExtraCastsAreBudgetFree &&
            DamageMultiplier == 1m && !TargetsAllEnemies && !RandomTargets &&
            ExtraDamageStrikes == 0 && DrawCount == 0m && !DrawForModifierEntries;

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

        /// <summary>乘以一个伤害倍率（如 0.8m 表示减少 20%）。</summary>
        public void MultiplyDamage(decimal factor)
        {
            DamageMultiplier *= factor;
        }

        /// <summary>把下一个法术的目标改为全体敌人。</summary>
        public void MarkTargetsAllEnemies()
        {
            TargetsAllEnemies = true;
        }

        public void MarkRandomTargets()
        {
            RandomTargets = true;
        }

        /// <summary>
        ///     复制一份当前累积状态。
        ///     <para>
        ///         用于【逆遍历】的倒序重放：**逆遍历那一刻器里剩下的，正好是「尚未被消费」的修正符**
        ///         （每次打完法术都会 <see cref="Reset" />），所以它们应当继续作用于重放的第一张法术。
        ///         </para>
        /// </summary>
        public MagicWineFoxSpellModifierState Copy()
        {
            return new MagicWineFoxSpellModifierState
            {
                DamageBonus = DamageBonus,
                ExtraCasts = ExtraCasts,
                ExtraCastsAreBudgetFree = ExtraCastsAreBudgetFree,
                DamageMultiplier = DamageMultiplier,
                TargetsAllEnemies = TargetsAllEnemies,
                RandomTargets = RandomTargets,
                ExtraDamageStrikes = ExtraDamageStrikes,
                DrawCount = DrawCount,
                DrawForModifierEntries = DrawForModifierEntries,
                WandModifierCount = WandModifierCount,
            };
        }

        /// <summary>登记本次释放时法杖内的修正符总数。</summary>
        public void SetWandModifierCount(int count)
        {
            WandModifierCount = count;
        }

        /// <summary>登记「额外结算几次伤害」。</summary>
        public void AddExtraDamageStrikes(int amount)
        {
            ExtraDamageStrikes += amount;
        }

        /// <summary>登记整轮抽牌效果。多次登记会叠加抽牌数；只要有一次升级，修正符条目也抽牌。</summary>
        public void MarkDrawPerSpell(decimal count, bool includeModifierEntries)
        {
            DrawCount += count;
            DrawForModifierEntries |= includeModifierEntries;
        }

        /// <summary>消费掉当前累积的修正。每施放一张法术后必须调用，否则修正会泄漏到后续法术。</summary>
        public void Reset()
        {
            DamageBonus = 0m;
            ExtraCasts = 0;
            ExtraCastsAreBudgetFree = false;
            DamageMultiplier = 1m;
            TargetsAllEnemies = false;
            RandomTargets = false;
            ExtraDamageStrikes = 0;
        }
    }
}
