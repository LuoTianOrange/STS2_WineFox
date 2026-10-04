using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class EternalMelodyPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.EternalMelodyPowerIcon);

        internal static async Task ApplyToSpellTargets(
            PlayerChoiceContext choiceContext,
            Player owner,
            CardModel spell,
            IEnumerable<Creature> attacked)
        {
            var power = owner.Creature?.Powers.OfType<EternalMelodyPower>().FirstOrDefault();
            if (power == null)
                return;

            var loss = power.Amount > 0m ? power.Amount : 4m;
            if (loss <= 0m)
                return;

            var targets = attacked.Where(enemy => enemy.IsAlive).Distinct().ToList();
            if (targets.Count == 0)
                return;

            power.Flash();

            foreach (var enemy in targets)
            {
                if (!enemy.IsAlive)
                    continue;

                await CreatureCmd.SetMaxAndCurrentHp(enemy, enemy.CurrentHp);

                await CreatureCmd.SetCurrentHp(enemy, Math.Max(0m, enemy.CurrentHp - loss));

                if (!enemy.IsAlive)
                    continue;

                if (enemy.CurrentHp <= 0m)
                {
                    await CreatureCmd.Damage(
                        new ThrowingPlayerChoiceContext(),
                        enemy,
                        enemy.MaxHp,
                        ValueProp.Unblockable | ValueProp.Unpowered,
                        null,
                        null);

                    continue;
                }

                await CreatureCmd.SetMaxAndCurrentHp(enemy, enemy.CurrentHp);
            }
        }
    }
}
