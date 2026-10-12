using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.VFX;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class GatheringLight() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(6m, ValueProp.Move),
            new("BonusPerCast", 3m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGatheringLight);

        public override string SpellIconPath => Const.Paths.SpellIconGatheringLight;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner || owner.Creature?.CombatState is not { } combatState)
                return;

            var targets = ResolveSpellTargets(context);
            if (targets.Count == 0)
                return;

            var bonus = DynamicVars["BonusPerCast"].BaseValue;

            var attack = DamageCmd.Attack(context.DamageWithModifiers(DynamicVars.Damage.BaseValue))
                .FromCard(context.SourceCard, null);

            if (owner.Creature is { } caster && targets[0] is { } beamTarget)
                GatheringLightVfx.Play(caster, beamTarget);

            if (context.TargetsAllEnemies)
                await attack.TargetingAllOpponents(combatState).Execute(context.ChoiceContext);
            else
                await attack.Targeting(targets[0]).Execute(context.ChoiceContext);

            var newBase = DynamicVars.Damage.BaseValue + bonus;
            DynamicVars.Damage.BaseValue = newBase;

            foreach (var card in owner.PlayerCombatState.AllCards.OfType<GatheringLight>())
                card.DynamicVars.Damage.BaseValue = newBase;
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["BonusPerCast"].UpgradeValueBy(1m);
        }
    }
}
