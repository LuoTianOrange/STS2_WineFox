using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class RadiationLeakPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.RadiationLeakIcon);

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            Flash();

            for (var i = 0; i < Amount + 1; i++)
            {
                var dazedCard = CombatState.CreateCard<Dazed>(player);
                var cardInstance = await CardPileCmd.AddGeneratedCardToCombat(dazedCard, PileType.Hand, true);
                CardCmd.PreviewCardPileAdd(cardInstance);
            }

            if (Amount < 10) await PowerCmd.Apply<RadiationLeakPower>(Owner, 1m, Owner, null);
        }
    }
}
