using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxPotionPool))]
    public sealed class EnchantedGoldenApple : SellableToMerchantPotionModel
    {
        protected override int SellGold => 128;
        public override PotionRarity Rarity => PotionRarity.Rare;
        protected override TargetType CombatTargetType => TargetType.Self;

        protected override async Task OnUseInCombat(PlayerChoiceContext choiceContext, Creature? target)
        {
            await CreatureCmd.Heal(Owner.Creature, 15);
            await PowerCmd.Apply<RegenPower>(choiceContext, Owner.Creature, 12, Owner.Creature, cardSource: null);
            await CreatureCmd.GainBlock(Owner.Creature, 15, ValueProp.Unpowered, cardPlay: null);
            await PowerCmd.Apply<HpLossReductionPower>(choiceContext, Owner.Creature, 2, Owner.Creature, cardSource: null);
        }
    }
}
