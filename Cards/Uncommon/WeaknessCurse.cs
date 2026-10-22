using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class WeaknessCurse() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellCard, ILimitedUseCard
    {
        private const int UsesLimit = 3;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Debuff", 1m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardWeaknessCurse);

        public override string SpellIconPath => Const.Paths.SpellIconWeaknessCurse;

        public override SpellTraits Traits =>
            new() { AttackSpell = false, SupportSpell = true, AreaOfEffect = true };

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<VulnerablePower>()
        ];

        [SavedProperty]
        public int UsesUsed { get; set; }

        public int MaxUses => UsesLimit;

        public bool IsUnlimitedUses =>
            IsMutable &&
            Owner?.Relics?.OfType<ILimitedUseModifier>()
                .Any(relic => relic.GrantsUnlimitedUses(this)) == true;

        public int UsesRemaining =>
            IsUnlimitedUses ? int.MaxValue : Math.Max(0, MaxUses - UsesUsed);

        public bool TryConsumeUse()
        {
            if (IsUnlimitedUses || UsesUsed >= MaxUses)
                return false;

            UsesUsed += 1;
            return UsesUsed >= MaxUses;
        }

        protected override void AddExtraArgsToDescription(LocString description)
        {
            base.AddExtraArgsToDescription(description);

            description.Add("Uses", IsUnlimitedUses ? "∞" : UsesRemaining.ToString());
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);

            if (TryConsumeUse())
                await CardCmd.Exhaust(choiceContext, this);
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner?.Creature is not { } caster ||
                caster.CombatState is not { } combatState)
                return;

            var amount = DynamicVars["Debuff"].BaseValue;

            foreach (var enemy in combatState.HittableEnemies.Where(enemy => enemy.IsAlive).ToList())
            {
                var hasWeak = (enemy.GetPower<WeakPower>()?.Amount ?? 0m) > 0m;

                if (hasWeak)
                {
                    await PowerCmd.Apply<VulnerablePower>(
                        context.ChoiceContext, enemy, amount, caster, context.SourceCard);
                }
                else
                {
                    await PowerCmd.Apply<WeakPower>(
                        context.ChoiceContext, enemy, amount, caster, context.SourceCard);
                }
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Debuff"].UpgradeValueBy(1m);
        }
    }
}
