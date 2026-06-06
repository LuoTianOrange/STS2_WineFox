using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class BatchCraft() : WineFoxCard(
        0, CardType.Skill, CardRarity.Rare, TargetType.None), ICraftingCard
    {
        protected override bool HasEnergyCostX => true;

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.CraftKeyword];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBatchCraft);

        protected override bool IsPlayable =>
            Owner?.Creature is { CombatState: not null } creature && CraftCmd.CanCraftAny(creature);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var x = ResolveEnergyXValue();
            var productCount = x <= 0
                ? 1
                : IsUpgraded ? x + 1 : x;
            var craftCount = Math.Max(1, x);

            await CraftCmd.CraftIntoHandMultipleFromSingleCost(
                choiceContext,
                this,
                productCount,
                craftCount);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
