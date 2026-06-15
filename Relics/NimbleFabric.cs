using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public class NimbleFabric : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Rare;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.NimbleFabricRelicIcon);

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            if (player.Creature.CombatState?.RoundNumber != 1) return;

            Flash();
            await PowerCmd.Apply<BufferPower>(Owner.Creature, 1m, Owner.Creature, null);
        }
    }
}
