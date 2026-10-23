using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib;

namespace STS2_WineFox.Combat
{
    internal static class SpellSlotCombatSetup
    {
        private static IDisposable? _subscription;

        public static void Initialize()
        {
            _subscription ??= RitsuLibFramework.SubscribeLifecycle<CombatStartingEvent>(OnCombatStarting);
        }

        private static void OnCombatStarting(CombatStartingEvent combatStarting)
        {
            if (combatStarting.CombatState is not { } combatState)
                return;

            foreach (var player in combatState.RunState.Players)
            {
                if (player.Character is not MagicWineFox)
                    continue;

                _ = MagicWineFoxSpellCmd.EnsurePower(player);
            }
        }
    }
}
