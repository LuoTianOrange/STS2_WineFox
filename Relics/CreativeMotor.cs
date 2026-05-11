using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class CreativeMotor : WineFoxRelic, IStressConsumeListener
    {
        private bool _usedThisTurn;

        public override RelicRarity Rarity => RelicRarity.Rare;

        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.CreativeMotorRelicIcon);

        public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player == Owner)
                _usedThisTurn = false;

            return Task.CompletedTask;
        }

        public async Task OnStressConsumed(Creature creature)
        {
            if (creature != Owner.Creature || _usedThisTurn)
                return;

            _usedThisTurn = true;
            Flash();
            await PowerCmd.Apply<StressPower>(
                new ThrowingPlayerChoiceContext(),
                creature,
                1m,
                creature,
                cardSource: null);
        }
    }
}

