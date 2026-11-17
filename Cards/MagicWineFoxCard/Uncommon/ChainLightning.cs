using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Cards.WineFoxCard;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class ChainLightning() : MagicWineFoxSpellCard(
        2, CardType.Attack, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(10m, ValueProp.Move),
            new PowerVar<VulnerablePower>(1m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardChainLightning);

        public override string SpellIconPath => Const.Paths.SpellIconChainLightning;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<VulnerablePower>()];

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner?.Creature is not { } ownerCreature ||
                ownerCreature.CombatState is not { } combatState)
                return;

            var repeats = 1 + Math.Max(0, MagicWineFoxSpellCmd.CountLoadedOverloads(context.Owner));
            var rng = context.Owner.RunState.Rng.CombatTargets;

            for (var i = 0; i < repeats; i++)
            {
                var enemies = combatState.HittableEnemies.Where(enemy => enemy.IsAlive).ToList();
                if (enemies.Count == 0)
                    return;

                var target = enemies[rng.NextInt(enemies.Count)];

                await DamageCmd.Attack(context.DamageWithModifiers(DynamicVars.Damage.BaseValue))
                    .FromCard(context.SourceCard, null)
                    .Targeting(target)
                    .Execute(context.ChoiceContext);

                await PowerCmd.Apply<VulnerablePower>(
                    new ThrowingPlayerChoiceContext(),
                    target,
                    DynamicVars.Vulnerable.BaseValue,
                    ownerCreature,
                    context.SourceCard);
            }
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
            DynamicVars.Vulnerable.UpgradeValueBy(1m);
        }
    }
}
