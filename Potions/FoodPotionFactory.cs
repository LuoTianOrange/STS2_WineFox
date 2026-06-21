using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using STS2_WineFox.Character;
using STS2_WineFox.Settings;

namespace STS2_WineFox.Potions
{
    internal static class FoodPotionFactory
    {
        private const float RareThreshold = 0.1f;
        private const float UncommonThreshold = 0.35f;

        public static PotionModel? CreateRandomFoodPotionForReward(Player player, Rng rng, IEnumerable<PotionModel>? blacklist = null)
        {
            if (!WineFoxRuntimeSettings.FoodEnabled)
                return null;

            blacklist ??= Array.Empty<PotionModel>();

            var options = ModelDb
                .PotionPool<WineFoxFoodPotionPool>()
                .GetUnlockedPotions(player.UnlockState)
                .Where(p => !blacklist.Contains(p))
                .ToList();

            if (options.Count == 0)
                return null;

            var roll = rng.NextFloat();
            var rarity =
                roll <= RareThreshold ? PotionRarity.Rare :
                roll <= UncommonThreshold ? PotionRarity.Uncommon :
                PotionRarity.Common;

            var picked = TryPickByRarity(options, rarity, rng)
                ?? TryPickByRarity(options, PotionRarity.Uncommon, rng)
                ?? TryPickByRarity(options, PotionRarity.Common, rng)
                ?? TryPickByRarity(options, PotionRarity.Rare, rng);

            return picked;
        }

        private static PotionModel? TryPickByRarity(IReadOnlyList<PotionModel> options, PotionRarity rarity, Rng rng)
        {
            var candidates = options.Where(p => p.Rarity == rarity).ToList();
            return candidates.Count == 0 ? null : rng.NextItem(candidates);
        }
    }
}

