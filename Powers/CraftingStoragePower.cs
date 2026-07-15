using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class CraftingStoragePower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.CraftingStoragePowerIcon);

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            var products = CombatManager.Instance.History.Entries
                .OfType<CardGeneratedEntry>()
                .Where(entry =>
                    entry.Creator == player &&
                    entry.HappenedLastPlayerTurn(player) &&
                    CraftCmd.IsCraftHandProduct(entry.Card))
                .Select(entry => entry.Card)
                .ToList();
            if (products.Count == 0) return;

            Flash();
            foreach (var product in products)
                for (var i = 0; i < Amount; i++)
                {
                    var clone = product.CreateClone();
                    await CardPileCmd.AddGeneratedCardToCombat(
                        clone,
                        PileType.Hand,
                        player);
                }
        }
    }
}
