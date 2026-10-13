using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class Lightsaber() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(3m, ValueProp.Move),
            new("Bonus", 2m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLightsaber);

        public override string SpellIconPath => Const.Paths.SpellIconLightsaber;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        public override SpellTraits Traits => new() { HitCount = 3 };

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner;
            var target = ResolveSpellTarget(context);
            if (owner?.Creature == null || target == null)
                return;

            var bonus = DynamicVars["Bonus"].BaseValue;
            var hits = ResolveHitCount(context);

            for (var hit = 0; hit < hits; hit++)
            {
                var blockBefore = target.Block;

                await DamageCmd.Attack(context.DamageWithModifiers(DynamicVars.Damage.BaseValue))
                    .FromCard(context.SourceCard, null)
                    .Targeting(target)
                    .Execute(context.ChoiceContext);

                if (!target.IsAlive)
                    break;

                if (target.Block >= blockBefore)
                    continue;

                await DamageCmd.Attack(context.DamageWithModifiers(bonus))
                    .FromCard(context.SourceCard, null)
                    .Targeting(target)
                    .Execute(context.ChoiceContext);
            }
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Bonus"].UpgradeValueBy(1m);
        }
    }
}
