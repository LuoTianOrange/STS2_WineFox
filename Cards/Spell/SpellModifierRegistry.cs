using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace STS2_WineFox.Cards.Spell
{
    /// <summary>一种可被随机生成的法术修正。</summary>
    public sealed record SpellModifierOption(
        Type CardType,
        Func<ICombatState, Player, CardModel> Factory);

    /// <summary>
    ///     「随机法术修正」的候选池。
    ///     <para>
    ///         新增法术修正符时，在 <see cref="All" /> 里登记一行即可被【全视之眼】抽到。
    ///         写法与 <c>CraftRecipeRegistry</c> 一致（类型 + 生成工厂）。
    ///     </para>
    /// </summary>
    public static class SpellModifierRegistry
    {
        public static readonly IReadOnlyList<SpellModifierOption> All =
        [
            new(typeof(DoubleReleaseSigil), (state, owner) => state.CreateCard<DoubleReleaseSigil>(owner)),
            new(typeof(PiercingBolt), (state, owner) => state.CreateCard<PiercingBolt>(owner)),
        ];

        /// <summary>按给定 RNG 抽一个法术修正。</summary>
        public static SpellModifierOption Roll(Rng rng)
        {
            return All[rng.NextInt(0, All.Count)];
        }
    }
}
