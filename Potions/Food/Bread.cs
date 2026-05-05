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
    public sealed class Bread : SellableToMerchantPotionModel
    {
        protected override int SellGold => 20;
        public override PotionRarity Rarity => PotionRarity.Uncommon;
        protected override TargetType CombatTargetType => TargetType.Self;

        protected override async Task OnUseInCombat(PlayerChoiceContext choiceContext, Creature? target)
        {
            await CreatureCmd.Heal(Owner.Creature, 6);
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }
}
