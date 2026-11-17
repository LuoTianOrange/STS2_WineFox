using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.VFX;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    [RegisterCharacterStarterCard(typeof(MagicWineFox), 4)]
    public class FoxfireBolt() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Basic, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(6m, ValueProp.Move),
        ];

        protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardFoxfireBolt);

        public override string SpellIconPath => Const.Paths.SpellIconFoxfireBolt;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            PlayBoltVfx(context);
            await DealSpellDamage(context, DynamicVars.Damage.BaseValue);
        }

        private static void PlayBoltVfx(MagicWineFoxSpellCastContext context)
        {
            var caster = context.Owner?.Creature;
            if (caster == null)
                return;

            var target = context.Target;
            if (target is not { IsAlive: true })
                target = caster.CombatState?.HittableEnemies.FirstOrDefault();

            if (target == null)
                return;

            FoxfireBoltVfx.Play(caster, target);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3m);
        }
    }
}
