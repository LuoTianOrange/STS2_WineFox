using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Combat.Magic
{
    public static class MagicBlock
    {
        public static decimal Resolve(CardModel card, decimal baseAmount)
        {
            var defender = card._owner?.Creature;
            if (defender == null) return Math.Max(0m, baseAmount);

            var additive = 0m;
            foreach (var modifier in defender.Powers.OfType<IMagicBlockModifier>())
                additive += modifier.ModifyMagicBlockAdditive(defender, baseAmount, card);

            return Math.Max(0m, baseAmount + additive);
        }
    }
}


