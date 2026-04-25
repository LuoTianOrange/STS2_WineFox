using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Enchantments
{
    [RegisterEnchantment]
    public class SweepingEdge : WineFoxEnchantmentsPool
    {
        private bool _isSweeping;

        public override EnchantmentAssetProfile AssetProfile => new(Const.Paths.EnchantmentSweepingEdgeIcon);

        public override bool ShowAmount => true;

        public override bool CanEnchantCardType(CardType cardType)
        {
            return cardType == CardType.Attack;
        }

        public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
        {
            if (_isSweeping || Card is null) return;
            if (!IsAttackFromEnchantedCard(command)) return;

            var ownerCreature = Card.Owner?.Creature;
            if (ownerCreature == null) return;
            if (ownerCreature.CombatState is not { } combatState) return;

            var pendingSweepDamage = new Dictionary<Creature, decimal>();
            var opponents = combatState.GetOpponentsOf(ownerCreature).Where(enemy => enemy.IsAlive).ToList();
            if (opponents.Count == 0) return;

            foreach (var result in command.Results)
            {
                if (result.Receiver is not { } receiver) continue;

                var resultDamage = result.TotalDamage + result.OverkillDamage;
                if (resultDamage <= 0) continue;

                var sweepDamage = Math.Ceiling(resultDamage * 0.5m);
                if (sweepDamage <= 0m) continue;

                foreach (var enemy in opponents)
                {
                    if (enemy == receiver) continue;

                    if (pendingSweepDamage.TryGetValue(enemy, out var existingDamage))
                        pendingSweepDamage[enemy] = existingDamage + sweepDamage;
                    else
                        pendingSweepDamage[enemy] = sweepDamage;
                }
            }

            if (pendingSweepDamage.Count == 0) return;

            _isSweeping = true;
            try
            {
                foreach (var (enemy, totalSweepDamage) in pendingSweepDamage)
                {
                    if (totalSweepDamage <= 0m) continue;
                    if (!enemy.IsAlive) continue;

                    await DamageCmd.Attack(totalSweepDamage)
                        .Unpowered()
                        .FromCard(Card)
                        .Targeting(enemy)
                        .WithHitFx("vfx/vfx_attack_slash")
                        .Execute(choiceContext);
                }
            }
            finally
            {
                _isSweeping = false;
            }
        }

        private bool IsAttackFromEnchantedCard(AttackCommand command)
        {
            if (command.ModelSource == Card) return true;
            if (command.ModelSource is not CardModel sourceCard) return false;

            return sourceCard.Owner == Card.Owner && sourceCard.Id == Card.Id;
        }
    }
}
