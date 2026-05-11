using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using STS2_WineFox.Character;
using STS2_WineFox.Potions;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class OrangeTigerCake : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Rare;
        public override bool HasUponPickupEffect => true;

        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.OrangeTigerCakeRelicIcon);

        public override async Task AfterObtained()
        {
            Flash();

            await CreatureCmd.GainMaxHp(Owner.Creature, 8m);
            await GrantRandomFoods(2);
            await PlayerCmd.GainGold(50, Owner);
            await OfferWineFoxCardRewardSet();
        }

        private async Task GrantRandomFoods(int count)
        {
            for (var i = 0; i < count; i++)
            {
                if (!Owner.HasOpenPotionSlots)
                    break;

                var potionModel = FoodPotionFactory.CreateRandomFoodPotionForReward(
                    Owner,
                    Owner.RunState.Rng.CombatPotionGeneration);
                if (potionModel == null)
                    break;

                var potion = potionModel.ToMutable();
                if (!(await PotionCmd.TryToProcure(potion, Owner)).success)
                    break;
            }
        }

        private async Task OfferWineFoxCardRewardSet()
        {
            var options = CardCreationOptions
                .ForNonCombatWithUniformOdds(
                    [Owner.Character.CardPool],
                    c => c.Rarity is CardRarity.Common or CardRarity.Uncommon or CardRarity.Rare)
                .WithFlags(CardCreationFlags.NoRarityModification);

            var rewards = new List<Reward>
            {
                new CardReward(options, 3, Owner)
            };

            await RewardsCmd.OfferCustom(Owner, rewards);
        }
    }
}



