using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Combat.Magic
{
    public static class MagicDamage
    {
        public static decimal Resolve(CardModel card, decimal baseAmount, Creature? target = null)
        {
            var dealer = card._owner?.Creature;
            if (dealer == null) return Math.Max(0m, baseAmount);

            var additive = 0m;
            foreach (var modifier in dealer.Powers.OfType<IMagicDamageModifier>())
                additive += modifier.ModifyMagicDamageAdditive(target, baseAmount, dealer, card);

            return Math.Max(0m, baseAmount + additive);
        }
    }
}

