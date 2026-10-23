using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class Eclipse() : MagicWineFoxSpellCard(
        2, CardType.Skill, CardRarity.Rare, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(14m, ValueProp.Move),
            new("StealPercent", 50m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEclipse);

        public override string SpellIconPath => Const.Paths.SpellIconEclipse;

        public override SpellTraits Traits => new() { AttackSpell = false, TargetSelf = true };

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner?.Creature is not { } creature ||
                creature.CombatState is not { } combatState)
                return;

            await CreatureCmd.GainBlock(
                creature,
                context.BlockWithModifiers(DynamicVars.Block.BaseValue),
                ValueProp.Move,
                null);

            var percent = DynamicVars["StealPercent"].BaseValue / 100m;
            var stolen = 0m;

            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                var take = Math.Floor(enemy.Block * percent);

                if (take <= 0m)
                    continue;

                stolen += take;
                await CreatureCmd.LoseBlock(context.ChoiceContext, enemy, take, creature);
            }

            if (stolen > 0m)
                await CreatureCmd.GainBlock(creature, stolen, ValueProp.Unpowered, null);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["StealPercent"].UpgradeValueBy(25m);
        }
    }
}
