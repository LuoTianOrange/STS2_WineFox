using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     法术余烬：法杖中的攻击型法术被释放时，给所有敌人叠加灼烧。
    /// </summary>
    [RegisterPower]
    public class SpellAshesPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.SpellAshesPowerIcon);
        public decimal BurnPerCast => Amount;
        
        public static async Task ApplyToAllEnemies(
            PlayerChoiceContext choiceContext,
            Player owner,
            CardModel? sourceCard)
        {
            if (owner.Creature is not { } creature)
                return;

            var ashes = creature.Powers.OfType<SpellAshesPower>().FirstOrDefault();
            if (ashes == null || ashes.BurnPerCast <= 0m)
                return;

            if (creature.CombatState is not { } combatState)
                return;

            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                if (!enemy.IsAlive)
                    continue;

                await PowerCmd.Apply<BurningPower>(
                    choiceContext,
                    enemy,
                    ashes.BurnPerCast,
                    creature,
                    sourceCard);
            }
        }
    }
}
