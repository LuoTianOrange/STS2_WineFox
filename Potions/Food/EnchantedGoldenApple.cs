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
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxFoodPotionPool))]
    public sealed class EnchantedGoldenApple : SellableToMerchantPotionModel
    {
        protected override int SellGold => 50;
        public override PotionRarity Rarity => PotionRarity.Rare;
        public override bool CanBeGeneratedInCombat => false;

        public override PotionAssetProfile AssetProfile => Art(Const.Paths.EnchantedGoldenApple);

        protected override async Task OnUseInCombat(PlayerChoiceContext choiceContext, Creature? target)
        {
            var targetCreature = GetCombatTarget(target);
            await CreatureCmd.Heal(targetCreature, 4);
            await PowerCmd.Apply<RegenPower>(choiceContext, targetCreature, 12, Owner.Creature, cardSource: null);
            await CreatureCmd.GainBlock(targetCreature, 15, ValueProp.Unpowered, cardPlay: null);
            await PowerCmd.Apply<HpLossReductionPower>(choiceContext, targetCreature, 2, Owner.Creature, cardSource: null);
        }

        protected override Task OnUseOutOfCombat(PlayerChoiceContext choiceContext) =>
            CreatureCmd.Heal(Owner.Creature, 4);
    }
}

