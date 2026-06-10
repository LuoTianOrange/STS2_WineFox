using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class PlanningExpertPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.PlanningExpertPowerIcon);

        public override async Task BeforeSideTurnEndEarly(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (side != Owner.Side) return;
            if (Owner.Player == null) return;

            var player = Owner.Player;
            var handCount = PileType.Hand.GetPile(player).Cards.Count;
            if (handCount == 0) return;

            var maxSelect = Math.Min(Amount, handCount);
            var prompt = new LocString("powers", "STS2_WINE_FOX_POWER_PLANNING_EXPERT_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 0, maxSelect);
            var selectedList = await CardSelectCmd.FromHandForDiscard(choiceContext, player, prefs, null, null!);

            Flash();
            foreach (var card in selectedList)
                card.AddKeyword(CardKeyword.Retain);
        }
    }
}
