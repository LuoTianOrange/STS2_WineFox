using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxPotionPool))]
    public sealed class Watermelon : SellableToMerchantPotionModel
    {
        protected override int SellGold => 5;
        public override PotionRarity Rarity => PotionRarity.Common;
        protected override TargetType CombatTargetType => TargetType.Self;

        protected override Task OnUseInCombat(PlayerChoiceContext choiceContext, Creature? target) =>
            CreatureCmd.Heal(Owner.Creature, 2);
    }
}
