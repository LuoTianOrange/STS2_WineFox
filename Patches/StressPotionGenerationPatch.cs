using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Potions;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    internal sealed class StressPotionGenerationPatch : IPatchMethod
    {
        public static string PatchId => "winefox_stress_potion_character_filter";

        public static string Description =>
            "Only let the WineFox character roll the Stress potion";

        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets() =>
        [
            new(typeof(PotionFactory), "GetPotionOptions", null, true),
        ];

        public static void Postfix(Player player, ref IEnumerable<PotionModel> __result)
        {
            try
            {
                if (player?.Character is WineFox)
                    return;

                __result = __result.Where(potion => potion is not StressPotion).ToList();
            }
            catch
            {
            }
        }
    }
}
