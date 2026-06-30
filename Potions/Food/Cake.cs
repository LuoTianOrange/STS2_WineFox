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
        public override bool CanBeGeneratedInCombat => false;

        public override PotionAssetProfile AssetProfile => Art(Const.Paths.Cake);

        protected override Task OnUseInCombat(PlayerChoiceContext choiceContext, Creature? target)
        {
            var targetCreature = GetCombatTarget(target);
            return targetCreature.Player == null
                ? Task.CompletedTask
                : ApplyCakeEffects(targetCreature);
        }

        protected override Task OnUseOutOfCombat(PlayerChoiceContext choiceContext)
        {
            return ApplyCakeEffects(Owner.Creature);
        }

        private async Task ApplyCakeEffects(Creature targetCreature)
        {
            await CreatureCmd.GainMaxHp(targetCreature, 14);

            var player = targetCreature.Player ?? Owner;
            var candidates = player.Deck.Cards.Where(card => card.IsUpgradable).ToList();
            if (candidates.Count == 0)
                return;

            var selected = Owner.RunState.Rng.CombatCardSelection.NextItem(candidates);
            if (selected != null)
                CardCmd.Upgrade(selected);
        }
    }
}
