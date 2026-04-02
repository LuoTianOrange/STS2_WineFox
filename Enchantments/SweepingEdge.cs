using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Enchantments
{
    public class SweepingEdge : WineFoxEnchantmentsPool
    {
        public override EnchantmentAssetProfile AssetProfile => new(Const.Paths.EnchantmentSweepingEdgeIcon);

        public override bool ShowAmount => true;

        public override bool CanEnchantCardType(CardType cardType)
        {
            return cardType == CardType.Attack;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card != Card) return;

            var explicitTarget = cardPlay.Target;
            if (explicitTarget == null) return;

            var ownerCreature = cardPlay.Card.Owner?.Creature;
            if (ownerCreature == null) return;
            if (ownerCreature.CombatState is not { } combatState) return;

            var baseDamage = 0m;
            if (cardPlay.Card.DynamicVars.TryGetValue("Damage", out var dv))
                baseDamage = dv.BaseValue;
            else
                return;

            var halfDamage = Math.Ceiling(baseDamage * 0.5m);
            if (halfDamage <= 0m) return;

            var others = combatState.HittableEnemies?.Where(e => !ReferenceEquals(e, explicitTarget)).ToList();
            if (others == null || others.Count == 0) return;

            foreach (var enemy in others)
                await DamageCmd.Attack(halfDamage)
                    .FromCard(cardPlay.Card)
                    .Targeting(enemy)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(context);
        }
    }
}
