using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    public class ShieldAttack() : WineFoxCard(
        0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
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
            var damage = block * 2m;

            await DamageCmd.Attack(damage)
                .FromCard(this)
                .TargetingAllOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            if (block > 0m) await CreatureCmd.LoseBlock(creature, block);

            await CardPileCmd.Draw(choiceContext, 1m, owner);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
