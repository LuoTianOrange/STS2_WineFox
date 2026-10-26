using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class NuclearWeapon() : MagicWineFoxSpellCard(
        0, CardType.Attack, CardRarity.Rare, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override bool HasEnergyCostX => true;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(12m, ValueProp.Move),
            new("Burn", 2m),
        ];

        public override SpellTraits Traits => new() { AreaOfEffect = true };

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNuclearWeapon);

        public override string SpellIconPath => Const.Paths.SpellIconNuclearWeapon;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<BurningPower>(),
        ];

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var x = context.Modifiers.CurrentX;
            if (x <= 0)
                return;

            var owner = context.Owner;
            if (owner?.Creature == null)
                return;

            await DealSpellDamage(context, DynamicVars.Damage.BaseValue * x);

            if (owner.Creature.CombatState is not { } combatState)
                return;

            var burn = DynamicVars["Burn"].BaseValue * x;

            foreach (var enemy in combatState.HittableEnemies.Where(enemy => enemy.IsAlive).ToList())
            {
                await PowerCmd.Apply<BurningPower>(
                    context.ChoiceContext,
                    enemy,
                    burn,
                    owner.Creature,
                    context.SourceCard);
            }
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var x = ResolveEnergyXValue();

            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, false, x);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
            DynamicVars["Burn"].UpgradeValueBy(1m);
        }
    }
}
