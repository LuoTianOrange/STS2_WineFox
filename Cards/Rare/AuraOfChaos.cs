using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class AuraOfChaos() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Rare, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Amount", 4m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAuraOfChaos);

        public override string SpellIconPath => Const.Paths.SpellIconAuraOfChaos;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<PoisonPower>(),
            HoverTipFactory.FromPower<BurningPower>(),
            HoverTipFactory.FromPower<DoomPower>(),
        ];

        public override SpellTraits Traits =>
            new() { AreaOfEffect = true };

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner || owner.Creature is not { } caster)
                return;

            if (owner.Creature.CombatState is not { } combatState)
                return;

            var energy = owner.PlayerCombatState?.Energy ?? 0;
            if (energy <= 0)
                return;

            await PlayerCmd.LoseEnergy(energy, owner);

            var amount = DynamicVars["Amount"].BaseValue;
            var enemies = combatState.HittableEnemies.Where(enemy => enemy.IsAlive).ToList();

            if (enemies.Count == 0)
                return;

            var everyEffect = energy >= 4;

            for (var i = 0; i < energy; i++)
            {
                if (everyEffect || i == 0)
                    await PowerCmd.Apply<PoisonPower>(
                        context.ChoiceContext, enemies, amount, caster, context.SourceCard);

                if (everyEffect || i == 1)
                    await PowerCmd.Apply<BurningPower>(
                        context.ChoiceContext, enemies, amount, caster, context.SourceCard);

                if (everyEffect || i == 2)
                    await PowerCmd.Apply<DoomPower>(
                        context.ChoiceContext, enemies, amount, caster, context.SourceCard);
            }
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Amount"].UpgradeValueBy(2m);
        }
    }
}
