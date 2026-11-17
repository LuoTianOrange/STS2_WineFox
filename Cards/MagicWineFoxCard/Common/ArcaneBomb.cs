using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
    public class ArcaneBomb() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(9m, ValueProp.Move),
        ];

        public override SpellTraits Traits => new() { AreaOfEffect = true };

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardArcaneBomb);

        public override string SpellIconPath => Const.Paths.SpellIconArcaneBomb;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            PlayBombVfx(context);
            await DealSpellDamage(context, DynamicVars.Damage.BaseValue);
        }

        private static void PlayBombVfx(MagicWineFoxSpellCastContext context)
        {
            var caster = context.Owner?.Creature;
            if (caster == null)
                return;

            var enemies = caster.CombatState?.HittableEnemies;
            if (enemies == null)
                return;

            var targets = new List<Creature>();

            if (context.Target is { IsAlive: true } single)
                targets.Add(single);
            else
                targets.AddRange(enemies.Where(enemy => enemy.IsAlive));

            if (targets.Count == 0)
                return;

            ArcaneBombVfx.Play(caster, targets);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }
    }
}
