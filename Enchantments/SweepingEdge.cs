using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using STS2_WineFox.Character;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Enchantments
{
    public class SweepingEdge : WineFoxEnchantmentsPool
    {
        private bool _isSweeping;

        public override EnchantmentAssetProfile AssetProfile => new(Const.Paths.EnchantmentSweepingEdgeIcon);

        public override bool ShowAmount => true;

        public override bool CanEnchantCardType(CardType cardType)
        {
            return cardType == CardType.Attack;
        }

        public override async Task AfterAttack(AttackCommand command)
        {
            if (_isSweeping || command.ModelSource != Card) return;

            var ownerCreature = Card?.Owner?.Creature;
            if (ownerCreature == null) return;
            if (ownerCreature.CombatState is not { } combatState) return;

            if (!Card.DynamicVars.TryGetValue("Damage", out var dv)) return;

            var halfDamage = Math.Ceiling(dv.BaseValue * 0.5m);
            if (halfDamage <= 0m) return;

            var hitTargets = new HashSet<Creature>();
            foreach (var result in command.Results)
                if (result.Receiver is { } receiver)
                    hitTargets.Add(receiver);

            var others = combatState.HittableEnemies?
                .Where(e => !hitTargets.Contains(e)).ToList();
            if (others == null || others.Count == 0) return;

            _isSweeping = true;
            try
            {
                foreach (var enemy in others)
                    await DamageCmd.Attack(halfDamage)
                        .FromCard(Card)
                        .Targeting(enemy)
                        .WithHitFx("vfx/vfx_attack_slash")
                        .Execute(null);
            }
            finally
            {
                _isSweeping = false;
            }
        }
    }
}
