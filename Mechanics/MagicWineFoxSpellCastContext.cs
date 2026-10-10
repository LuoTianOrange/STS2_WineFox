using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Combat.Magic;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Mechanics
{
    /// <summary>
    ///     单张法术被释放时的上下文。
    ///     <para><see cref="SourceCard" />：槽位里那张克隆法术本身。</para>
    ///     <para><see cref="CastSourceCard" />：触发本次释放的来源卡。</para>
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
        
        public bool TargetsAllEnemies => Modifiers.TargetsAllEnemies;

        public bool RandomTargets => Modifiers.RandomTargets;

        public int ExtraDamageStrikes => Modifiers.ExtraDamageStrikes;

        public int WandModifierCount => Modifiers.WandModifierCount;
        
        public bool IgnoresBlock => QuantumLockPower.IsActiveFor(Owner?.Creature);
        
        public decimal DamageWithModifiers(decimal baseDamage)
        {
            var raw = (baseDamage + Modifiers.DamageBonus) * Modifiers.DamageMultiplier;
            var afterModifiers = Math.Max(0m, Math.Round(raw, 0, MidpointRounding.AwayFromZero));
            
            return MagicDamage.Resolve(SourceCard, afterModifiers, Target);
        }
        
        public decimal BlockWithModifiers(decimal baseBlock)
        {
            var raw = (baseBlock + Modifiers.DamageBonus) * Modifiers.DamageMultiplier;
            var afterModifiers = Math.Max(0m, Math.Round(raw, 0, MidpointRounding.AwayFromZero));

            return MagicBlock.Resolve(SourceCard, afterModifiers);
        }
    }
}
