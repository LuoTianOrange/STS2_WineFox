using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class BatchCraft() : WineFoxCard(
        0, CardType.Skill, CardRarity.Rare, TargetType.None), ICraftingCard
    {
        protected override bool HasEnergyCostX => true;

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Craft];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBatchCraft);

        protected override bool IsPlayable =>
            Owner?.Creature is { CombatState: not null } creature && CraftCmd.CanCraftAny(creature);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var x = ResolveEnergyXValue();
            if (x <= 0)
                return;

            await CraftCmd.CraftIntoHandMultipleFromSingleCost(
                choiceContext,
                this,
                IsUpgraded ? x + 1 : x,
                x);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
