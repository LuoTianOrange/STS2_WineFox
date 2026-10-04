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

        /// <summary>本次施放是否应改为对所有敌人结算（由【穿刺魔弹】等修正符决定）。</summary>
        public bool TargetsAllEnemies => Modifiers.TargetsAllEnemies;

        public bool RandomTargets => Modifiers.RandomTargets;

        /// <summary>
        ///     结算伤害时使用：先加修正符的附加伤害，再乘伤害倍率（如穿刺魔弹的 -20%），最后取整。
        /// </summary>
        public decimal DamageWithModifiers(decimal baseDamage)
        {
            var raw = (baseDamage + Modifiers.DamageBonus) * Modifiers.DamageMultiplier;
            return Math.Max(0m, Math.Round(raw, 0, MidpointRounding.AwayFromZero));
        }
    }
}
