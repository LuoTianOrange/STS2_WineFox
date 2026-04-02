using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Cards.Token;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    public class LessHoliday() : WineFoxCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLessHoliday);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromCard<WorkWork>(IsUpgraded)];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var handCards = PileType.Hand.GetPile(owner).Cards;
            if (handCards.Count == 0) return;

            var maxSelect = Math.Min(3, handCards.Count);
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 0, maxSelect);
            var selectedList = await CardSelectCmd.FromSimpleGrid(choiceContext, handCards, owner, prefs);

            foreach (var chosen in selectedList)
            {
                var result = await CardCmd.TransformTo<WorkWork>(chosen);
                if (IsUpgraded && result.HasValue)
                    CardCmd.Upgrade(result.Value.cardAdded);
            }
        }

        protected override void OnUpgrade()
        {
        }
    }
}