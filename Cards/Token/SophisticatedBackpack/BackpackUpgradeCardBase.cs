using MegaCrit.Sts2.Core.Entities.Cards;
using STS2_WineFox.Commands;
using STS2_WineFox.Relics.Backpack.Effects;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.SophisticatedBackpack
{
    public abstract class BackpackUpgradeCardBase<TEffect>(string artPath) : WineFoxCard(
        -1, CardType.Status, CardRarity.Token, TargetType.None, false), ICraftChoiceEffect
        where TEffect : ISophisticatedBackpackEffect
    {
        public override bool CanBeGeneratedInCombat => false;
        public override int MaxUpgradeLevel => 0;
        public override CardAssetProfile AssetProfile => Art(artPath);

        public Task OnCraftChosen(CraftExecutionContext context)
        {
            context.Crafter.Player?.Relics
                .OfType<Relics.SophisticatedBackpack>()
                .FirstOrDefault()
                ?.TryApplyUpgrade<TEffect>(ApplyUpgradeValues);
            return Task.CompletedTask;
        }

        protected virtual void ApplyUpgradeValues(Relics.SophisticatedBackpack backpack)
        {
        }
    }
}
