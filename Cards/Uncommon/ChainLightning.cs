using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     链式闪电：击中目标后弹跳到另一个随机敌人。
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ChainLightning() : MagicWineFoxSpellCard(
        2, CardType.Attack, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(9m, ValueProp.Move),
            new("BounceDamage", 5m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardChainLightning);

        public override string SpellIconPath => Const.Paths.SpellIconChainLightning;

        public override SpellTraits Traits => new() { BounceTargets = 1 };

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<VulnerablePower>()];

        public override decimal PreviewDamage =>
            DynamicVars.Damage.BaseValue + DynamicVars["BounceDamage"].BaseValue;

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner?.Creature is not { } ownerCreature ||
                ownerCreature.CombatState is not { } combatState)
                return;

            var targets = ResolveSpellTargets(context);
            if (targets.Count == 0)
                return;

            List<Creature> attacked = [.. targets];

            await DealSpellDamage(context, DynamicVars.Damage.BaseValue);

            for (var i = 0; i < context.BounceTargets; i++)
            {
                // 弹跳：从其他存活敌人里随机挑一个。
                var others = combatState.HittableEnemies
                    .Where(enemy => enemy.IsAlive && !attacked.Contains(enemy))
                    .ToList();

                if (others.Count == 0)
                    break;

                var bounceTarget = others[context.Owner.RunState.Rng.CombatTargets.NextInt(others.Count)];
                attacked.Add(bounceTarget);

                await DamageCmd.Attack(context.DamageWithModifiers(DynamicVars["BounceDamage"].BaseValue))
                    .FromCard(context.SourceCard, null)
                    .Targeting(bounceTarget)
                    .Execute(context.ChoiceContext);
            }

            await PowerCmd.Apply<VulnerablePower>(
                new ThrowingPlayerChoiceContext(),
                attacked,
                1m,
                ownerCreature,
                context.SourceCard);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["BounceDamage"].UpgradeValueBy(2m);
        }
    }
}
