using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     单张法术被释放时的上下文。
    ///     <para><see cref="SourceCard" />：槽位里那张克隆法术本身。</para>
    ///     <para><see cref="CastSourceCard" />：触发本次释放的来源卡（可能是法杖/遗物，也可能为 null）。</para>
    ///     <para><see cref="Modifiers" />：本次施放吃到的修正符累积结果。</para>
    /// </summary>
    public sealed class MagicWineFoxSpellCastContext(
        PlayerChoiceContext choiceContext,
        Player owner,
        Creature? target,
        CardModel sourceCard,
        CardModel? castSourceCard,
        MagicWineFoxSpellModifierState modifiers)
    {
        public PlayerChoiceContext ChoiceContext { get; } = choiceContext;
        public Player Owner { get; } = owner;
        public Creature? Target { get; } = target;
        public CardModel SourceCard { get; } = sourceCard;
        public CardModel? CastSourceCard { get; } = castSourceCard;
        public MagicWineFoxSpellModifierState Modifiers { get; } = modifiers;

        /// <summary>结算伤害时使用：基础值 + 修正符附加伤害。</summary>
        public decimal DamageWithModifiers(decimal baseDamage)
        {
            return Math.Max(0m, baseDamage + Modifiers.DamageBonus);
        }
    }
}
