using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
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
                var potion = PotionFactory
                    .CreateRandomPotionOutOfCombat(Owner, Owner.RunState.Rng.CombatPotionGeneration)
                    .ToMutable();
                if (!(await PotionCmd.TryToProcure(potion, Owner)).success)
                    break;
            }
        }
    }
}
