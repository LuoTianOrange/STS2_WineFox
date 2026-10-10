using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class GravitationalSingularity() : MagicWineFoxSpellCard(
        2, CardType.Attack, CardRarity.Rare, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(18m, ValueProp.Move)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGravitationalSingularity);

        public override string SpellIconPath => Const.Paths.SpellIconGravitationalSingularity;
        
        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;
        
        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner?.Creature?.CombatState is not { } combatState)
                return;

            var targets = ResolveSpellTargets(context);
            if (targets.Count == 0)
                return;

            var attack = DamageCmd.Attack(context.DamageWithModifiers(DynamicVars.Damage.BaseValue))
                .FromCard(context.SourceCard, null);

            if (context.TargetsAllEnemies)
                await attack.TargetingAllOpponents(combatState).Execute(context.ChoiceContext);
            else
                await attack.Targeting(targets[0]).Execute(context.ChoiceContext);
            
            foreach (var enemy in targets.Where(e => e.IsAlive).ToList())
            {
                if (enemy.HasPower<GravitationalSingularityPower>())
                    continue;

                await PowerCmd.Apply<GravitationalSingularityPower>(
                    context.ChoiceContext,
                    enemy,
                    1m,
                    context.Owner.Creature,
                    context.SourceCard);
            }
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3);
        }
    }
}
