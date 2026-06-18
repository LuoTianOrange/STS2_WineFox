using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    [RegisterPotion(typeof(WineFoxFoodPotionPool))]
    public sealed class Cake : SellableToMerchantPotionModel
    {
        protected override int SellGold => 100;
        public override PotionRarity Rarity => PotionRarity.Rare;
        protected override TargetType CombatTargetType => TargetType.Self;
        public override bool CanBeGeneratedInCombat => false;

        public override PotionAssetProfile AssetProfile => Art(Const.Paths.Cake);

        protected override Task OnUseInCombat(PlayerChoiceContext choiceContext, Creature? target)
        {
            return ApplyCakeEffects();
        }

        protected override Task OnUseOutOfCombat(PlayerChoiceContext choiceContext)
        {
            return ApplyCakeEffects();
        }

        private async Task ApplyCakeEffects()
        {
            await CreatureCmd.GainMaxHp(Owner.Creature, 14);

            var candidates = Owner.Deck.Cards.Where(card => card.IsUpgradable).ToList();
            if (candidates.Count == 0)
                return;

            var selected = Owner.RunState.Rng.CombatCardSelection.NextItem(candidates);
            if (selected != null)
                CardCmd.Upgrade(selected);
        }
    }
}
