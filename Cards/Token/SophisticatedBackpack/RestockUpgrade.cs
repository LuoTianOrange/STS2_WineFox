using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Commands;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    public class RestockUpgrade() : WineFoxCard(
        -1, CardType.Status, CardRarity.Token, TargetType.None, showInCardLibrary: false), ICraftChoiceEffect
    {
        public static CraftDeliveryMode CraftProductDeliveryMode => CraftDeliveryMode.ImmediateEffect;

        public override bool CanBeGeneratedInCombat => false;
        public override int MaxUpgradeLevel => 0;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRestockUpgrade);

        public Task OnCraftChosen(CraftExecutionContext context)
        {
            TryApplyToBackpack(context.Crafter.Player);
            return Task.CompletedTask;
        }

        private static void TryApplyToBackpack(Player? player)
        {
            player?.Relics.OfType<Relics.SophisticatedBackpack>().FirstOrDefault()?.ApplyRestockUpgrade();
        }
    }
}
