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
    [RegisterRelic(typeof(WineFoxSharedRelicPool))]
    public class CatBomb : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Common;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.CatBombRelicIcon);

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            if (player.PlayerCombatState?.TurnNumber != 1) return;

            Flash();
            var bomb = await PowerCmd.Apply<TheBombPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 3m, Owner.Creature, null);
            bomb.SetDamage(30m);
        }
    }
}
