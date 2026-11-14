using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Potions;
using STS2_WineFox.Settings;
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

            var slotsBefore = Owner.MaxPotionCount;
            await PlayerCmd.GainMaxPotionCount(2, Owner);

            var pool = ModelDb.PotionPool<WineFoxFoodPotionPool>();

            var filled = 0;
            while (Owner.HasOpenPotionSlots)
            {
                var potionModel = FoodPotionFactory.CreateRandomFoodPotionForReward(
                    Owner,
                    Owner.RunState.Rng.CombatPotionGeneration);

                if (potionModel == null)
                {
                    break;
                }

                var result = await PotionCmd.TryToProcure(potionModel.ToMutable(), Owner);

                if (!result.success)
                    break;

                filled++;
            }

        }
    }
}
