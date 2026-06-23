using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace STS2_WineFox.Rewards
{
    internal sealed class FoodPotionReward(PotionModel potion, Player player) : PotionReward(potion, player)
    {
        public override SerializableReward ToSerializable()
        {
            var save = base.ToSerializable();
            save.PredeterminedModelId = Potion?.Id ?? save.PredeterminedModelId;
            return save;
        }
    }
}
