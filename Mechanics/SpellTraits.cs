namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     法术的通用属性声明。
    ///     <para>
    ///         属性只描述通用维度，卡牌的特殊逻辑仍留在各自的 CastAsSpell 里。
    ///     </para>
    /// </summary>
    public sealed record SpellTraits
    {
        public static SpellTraits Default { get; } = new();

        /// <summary>基础攻击段数。1表示单段</summary>
        public int HitCount { get; init; } = 1;

        /// <summary>攻击目标位全体</summary>
        public bool AreaOfEffect { get; init; }

        /// <summary>攻击型法术</summary>
        public bool AttackSpell { get; init; } = true;

        /// <summary>辅助型法术。自身向法术中非防御用途的归到这一类。</summary>
        public bool SupportSpell { get; init; }

        /// <summary>是否接受法术修正符修饰</summary>
        public bool AcceptsModifiers { get; init; } = true;

        /// <summary>是否吃修正符的额外段数</summary>
        public bool AcceptsExtraStrikes { get; init; } = true;

        /// <summary>是否计入本回合释放次数</summary>
        public bool CountsAsCast { get; init; } = true;

        /// <summary>是否可装填</summary>
        public bool Loadable { get; init; } = true;

        /// <summary>随机选择目标</summary>
        public bool RandomTarget { get; init; }

        /// <summary>以自身为目标</summary>
        public bool TargetSelf { get; init; }

        /// <summary>弹跳次数。0表示不弹跳</summary>
        public int BounceTargets { get; init; }

        /// <summary>本场战斗允许的装填次数。null 表示无限次</summary>
        public int? UsesLimit { get; init; }
    }
}
