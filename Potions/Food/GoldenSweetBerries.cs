using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxPotionPool))]
    public sealed class GoldenSweetBerries : SellableToMerchantPotionModel
    {
        protected override int SellGold => 30;
        public override PotionRarity Rarity => PotionRarity.Rare;
        protected override TargetType CombatTargetType => TargetType.Self;

        protected override async Task OnUseInCombat(PlayerChoiceContext choiceContext, Creature? target)
        {
            await CreatureCmd.Heal(Owner.Creature, 6);
            await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, 5, Owner.Creature, cardSource: null);
        }
    }
}
