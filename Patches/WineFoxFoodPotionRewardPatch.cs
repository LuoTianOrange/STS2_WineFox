using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2_WineFox.Potions;
using STS2_WineFox.Rewards;
using STS2_WineFox.Settings;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.RunData;

namespace STS2_WineFox.Patches
{
    public sealed class WineFoxFoodPotionRewardPatch : IPatchMethod
    {
        private const float BaseOdds = 0.4f;
        private const float TargetOdds = 0.5f;
        private const float EliteBonus = 0.25f;
        private const float IncrementStep = 0.1f;
        private const float LoseStep = 0.2f;

        // 该概率会影响后续奖励 RNG 消耗，必须写入当前局存档，避免 SL 后重置。
        private static readonly PlayerRunSavedData<FoodPotionRewardSaveState> FoodRewardState =
            RunSavedDataStore.For(Const.ModId).RegisterPerPlayer<FoodPotionRewardSaveState>(
                "food_potion_reward_odds",  // RitsuLib 已做隔离，不需要在此手动加前缀
                () => new FoodPotionRewardSaveState(),
                new() { WritePolicy = RunSavedDataWritePolicy.WhenNonDefault });

        internal static void InitializeSavedData()
        {
            _ = FoodRewardState;
        }

        public static string PatchId => "winefox_food_potion_reward_roll";
        public static bool IsCritical => true;
        public static string Description => "Adds separate food potion drop roll";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(RewardsSet), nameof(RewardsSet.WithRewardsFromRoom))];
        }

        // ReSharper disable once InconsistentNaming
        public static void Postfix(RewardsSet __instance, AbstractRoom room)
        {
            if (!WineFoxRuntimeSettings.FoodEnabled)
                return;

            if (room is not CombatRoom)
                return;

            var player = __instance.Player;
            if (player?.RunState == null)
                return;

            // Shops are not combat rooms, but keep this explicit.
            if (room.RoomType is not (RoomType.Monster or RoomType.Elite or RoomType.Boss))
                return;

            // Mirror vanilla gating: the final-act boss room produces no rewards.
            // (Important for scenarios like double-boss chains where RewardsSet.WithRewardsFromRoom returns early.)
            if (room.RoomType == RoomType.Boss
                && player.RunState.CurrentActIndex >= player.RunState.Acts.Count - 1)
                return;

            // Require at least one WineFox in the party for food to drop.
            if (player.Creature == null || !WineFoxCombatVisualEffects.TryIsWineFoxInParty(player.Creature))
                return;

            var rng = player.PlayerRng.Rewards;

            var current = FoodRewardState.Get(player).Odds;
            var roll = rng.NextFloat();

            var bonus = room.RoomType == RoomType.Elite ? EliteBonus : 0f;
            var success = roll < current + bonus * TargetOdds;

            if (!success)
            {
                SetFoodOdds(player, current + IncrementStep);
                return;
            }

            var potion = FoodPotionFactory.CreateRandomFoodPotionForReward(player, rng)?.ToMutable();
            if (potion == null)
                return;

            __instance.Rewards.Add(new FoodPotionReward(potion, player));
            SetFoodOdds(player, current - LoseStep);
        }

        private static void SetFoodOdds(Player player, float value)
        {
            var normalized = NormalizeOdds(value);
            FoodRewardState.Modify(player, data => data.Odds = normalized);
        }

        private static float NormalizeOdds(float value)
        {
            return Math.Clamp(value, 0f, 1f);
        }

        public sealed class FoodPotionRewardSaveState
        {
            public float Odds { get; set; } = BaseOdds;
        }
    }

    public sealed class WineFoxPredeterminedPotionRewardSavePatch : IPatchMethod
    {
        public static string PatchId => "winefox_predetermined_potion_reward_save";
        public static bool IsCritical => true;

        public static string Description =>
            "Restores serialized predetermined potion rewards without rerolling rewards rng";

        public static ModPatchTarget[] GetTargets()
        {
            return [new(typeof(Reward), nameof(Reward.FromSerializable))];
        }

        // ReSharper disable once InconsistentNaming
        public static bool Prefix(SerializableReward save, Player player, ref Reward __result)
        {
            if (save.RewardType != RewardType.Potion || save.PredeterminedModelId == ModelId.none)
                return true;

            var potion = ModelDb.GetById<PotionModel>(save.PredeterminedModelId).ToMutable();
            __result = new PotionReward(potion, player);
            return false;
        }
    }
}
