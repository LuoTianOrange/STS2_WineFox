using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using STS2_WineFox.Character;
using STS2_WineFox.Potions;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    [WineFoxPublicRelic]
    public class MaidBackpack : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Uncommon;
        public override bool HasUponPickupEffect => true;

        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.MaidBackpackRelicIcon);

        public override async Task AfterObtained()
        {
            Flash();

            await PlayerCmd.GainMaxPotionCount(2, Owner);

            while (Owner.HasOpenPotionSlots)
            {
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
    }
}
