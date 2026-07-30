using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Unlocks;
using STS2_WineFox.Character;
using STS2_WineFox.Relics;
using STS2_WineFox.Settings;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    public sealed class WineFoxPublicRelicRewardPoolPatch : IPatchMethod
    {
        public static string PatchId => "winefox_public_relic_reward_pool";
        public static bool IsCritical => true;
        public static string Description => "Adds enabled public WineFox relics to shared relic rewards";

        public static ModPatchTarget[] GetTargets()
        {
            return
            [
                new(
                    typeof(SharedRelicPool),
                    nameof(SharedRelicPool.GetUnlockedRelics),
                    [typeof(UnlockState)])
            ];
        }

        public static void Postfix(UnlockState unlockState, ref IEnumerable<RelicModel> __result)
        {
            if (!WineFoxRuntimeSettings.PublicRelicsEnabled)
                return;

            var publicRelics = ModelDb
                .RelicPool<WineFoxRelicPool>()
                .GetUnlockedRelics(unlockState)
                .Where(WineFoxPublicRelicAttribute.IsDefined);

            __result = __result
                .Concat(publicRelics)
                .DistinctBy(relic => relic.Id);
        }
    }
}
