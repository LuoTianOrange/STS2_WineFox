using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxFoodPotionPool))]
    public sealed class Beet : SellableToMerchantPotionModel, ICampfireTransformPotion
    {
        protected override int SellGold => 5;
        public override PotionRarity Rarity => PotionRarity.Common;
        public override bool CanBeGeneratedInCombat => false;

        public override PotionAssetProfile AssetProfile => Art(Const.Paths.Beet);

        public PotionModel CampfireTransformResult => ModelDb.Potion<BeetSoup>();
        
        protected override Task OnUseInCombat(PlayerChoiceContext choiceContext, Creature? target) =>
            CreatureCmd.Heal(GetCombatTarget(target), 1);

        protected override Task OnUseOutOfCombat(PlayerChoiceContext choiceContext) =>
            CreatureCmd.Heal(Owner.Creature, 1);
    }
}

