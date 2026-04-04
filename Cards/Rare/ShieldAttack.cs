using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    public class ShieldAttack() : WineFoxCard(
        0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new ("Attack",2m)];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardShieldAttack);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var creature = owner.Creature;

            var combatState = creature.CombatState;
            if (combatState is null) return;

            var block = creature.Block;
            var damage = block * DynamicVars["Attack"].BaseValue;

            await CreatureCmd.Damage(
                choiceContext,
                combatState.Enemies.Where(e => e.IsAlive),
                damage,
                ValueProp.Unpowered,
                creature,
                this);

            if (block > 0m) await CreatureCmd.LoseBlock(creature, block);

            await CardPileCmd.Draw(choiceContext, 1m, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Attack"].UpgradeValueBy(1m);
        }
    }
}
