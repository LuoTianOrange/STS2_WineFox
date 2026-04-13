using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using STS2_WineFox.Cards.Token.LessHoliday;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    public class LessHoliday() : WineFoxCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLessHoliday);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromCard<WorkWork>(IsUpgraded)];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var handCount = PileType.Hand.GetPile(owner).Cards.Count;
            if (handCount == 0) return;

            var maxSelect = Math.Min(3, handCount);
            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_LESS_HOLIDAY_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 0, maxSelect);
            var selectedList = await CardSelectCmd.FromHandForDiscard(choiceContext, owner, prefs, null, this);

            foreach (var chosen in selectedList)
            {
                if (chosen.CardScope == null) continue;
                var replacement = chosen.CardScope.CreateCard<WorkWork>(chosen.Owner);
                if (IsUpgraded)
                    CardCmd.Upgrade(replacement);

                await CardCmd.Transform(chosen, replacement);
            }
        }

        protected override void OnUpgrade()
        {
        }
    }
}
