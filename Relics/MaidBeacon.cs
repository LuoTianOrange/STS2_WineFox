using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(MagicWineFoxRelicPool))]
    // 起始遗物改为【狐火杖】(MagicWineFoxWand)；MaidBeacon 保留在遗物池中作为普通遗物。
    [RegisterTouchOfOrobasRefinement(typeof(Shrine))]
    public class MaidBeacon : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Common;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.MaidBeaconRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(1),
            new CardsVar(1),
        ];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            if (player.PlayerCombatState?.TurnNumber != 1) return;

            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }
}
