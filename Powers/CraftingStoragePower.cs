using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class CraftingStoragePower : WineFoxPower
    {
        private readonly List<CardModel> CurrentTurnProducts = [];
        private readonly List<CardModel> PreviousTurnProducts = [];

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.CraftingStoragePowerIcon);

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;
            if (PreviousTurnProducts.Count == 0) return;

            Flash();
            foreach (var product in PreviousTurnProducts)
                for (var i = 0; i < Amount; i++)
                {
                    var clone = product.CreateClone();
                    var cardInstance = await CardPileCmd.AddGeneratedCardToCombat(
                        clone,
                        PileType.Hand,
                        player,
                        CardPilePosition.Bottom);
                    CardCmd.PreviewCardPileAdd(cardInstance);
                }

            PreviousTurnProducts.Clear();
        }

        public override Task AfterCraftProductDelivered(CraftExecutionContext context)
        {
            if (context.Crafter != Owner) return Task.CompletedTask;
            if (context.Product == null) return Task.CompletedTask;
            if (context.DeliveryMode != CraftDeliveryMode.ToHand) return Task.CompletedTask;

            CurrentTurnProducts.Add(context.Product);
            return Task.CompletedTask;
        }

        public override Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return Task.CompletedTask;

            PreviousTurnProducts.Clear();
            PreviousTurnProducts.AddRange(CurrentTurnProducts);
            CurrentTurnProducts.Clear();
            return Task.CompletedTask;
        }
    }
}
