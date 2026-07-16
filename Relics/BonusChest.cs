using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public class BonusChest : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Common;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.BonusChestRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Iron", 3m)
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<IronPower>()];

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner) return;
            if (player.PlayerCombatState?.TurnNumber != 1) return;

            Flash();
            await PowerCmd.Apply<IronPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
                DynamicVars["Iron"].BaseValue,
                Owner.Creature,
                null);
        }
    }
}
