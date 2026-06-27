using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics.Event
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class RingofBurningBlood : WineFoxRelic
    {
        private const int MaxBonusPercent = 50;

        public override RelicRarity Rarity => RelicRarity.Event;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.RingofBurningBloodRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DynamicVar("MaxBonus", MaxBonusPercent)];

        public override decimal ModifyDamageMultiplicative(
            Creature? target,
            decimal amount,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (!props.IsPoweredAttack())
                return 1m;

            if (dealer != Owner.Creature && dealer != Owner.Osty)
                return 1m;

            var creature = Owner.Creature;
            if (creature.MaxHp <= 0)
                return 1m;

            var missingHpRatio = (decimal)(creature.MaxHp - creature.CurrentHp) / creature.MaxHp;
            var bonus = Math.Min(MaxBonusPercent / 100m, missingHpRatio);
            return 1m + bonus;
        }
    }
}
