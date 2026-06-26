using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;
using STS2_WineFox.Scripts.Effects;
using STS2RitsuLib.Combat.CardTargeting;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    public abstract class WineFoxPotion : ModPotionTemplate
    {
        public override PotionUsage Usage => PotionUsage.CombatOnly;

        protected virtual Color PotionParticleColor => Colors.White;

        protected IReadOnlyList<Creature> GetPotionHitTargets(Creature? selectedTarget = null)
        {
            return this.GetTargets(selectedTarget)
                .Where(target => target.IsAlive)
                .ToList();
        }

        protected IReadOnlyList<Creature> GetEnemyPotionTargets(Creature? selectedTarget = null)
        {
            var ownerCreature = Owner.Creature;
            return GetPotionHitTargets(selectedTarget)
                .Where(target => target.Side != ownerCreature.Side)
                .ToList();
        }

        protected void ShowPotionHitVfx(Creature? selectedTarget = null)
        {
            if (TestMode.IsOn || NCombatRoom.Instance == null)
                return;

            foreach (var target in GetPotionHitTargets(selectedTarget))
                ShowPotionHitVfxAt(target);
        }

        protected void ShowEnemyPotionHitVfx(Creature? selectedTarget = null)
        {
            if (TestMode.IsOn || NCombatRoom.Instance == null)
                return;

            foreach (var target in GetEnemyPotionTargets(selectedTarget))
                ShowPotionHitVfxAt(target);
        }

        private void ShowPotionHitVfxAt(Creature target)
        {
            var targetNode = NCombatRoom.Instance?.GetCreatureNode(target);
            if (targetNode == null)
                return;

            VfxPotion.Create(PotionParticleColor, targetNode.VfxSpawnPosition);
        }

        protected static PotionAssetProfile Art(string imagePath, string? outlinePath = null)
        {
            return new(imagePath, outlinePath ?? imagePath);
        }
    }
}
