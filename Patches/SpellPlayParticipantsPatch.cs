using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Patching.Models;

namespace STS2_WineFox.Patches
{
    internal sealed class SpellPlayParticipantsPatch : IPatchMethod
    {
        public static string PatchId => "winefox_spell_play_participants";

        public static string Description =>
            "Collect block modifiers touched by a spell release so they receive AfterCardPlayed";

        public static bool IsCritical => false;

        public static ModPatchTarget[] GetTargets() =>
        [
            new(typeof(Hook), "AfterModifyingBlockAmount", null, true),
        ];

        public static void Prefix(IEnumerable<AbstractModel> modifiers)
        {
            try
            {
                SpellPlayParticipants.Record(modifiers);
            }
            catch
            {
            }
        }
    }
}
