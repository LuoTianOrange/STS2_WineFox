using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Applied by OtherworldCrossing (异界跨越).
    ///     Every 2 turns, choose cards in your hand and add Ethereal + Exhaust copies.
    ///     Copies created each trigger equals Amount.
    /// </summary>
    [RegisterPower]
    public class OtherworldCrossingPower : WineFoxPower,
        IPowerExtraIconAmountLabelSpecsProvider,
        IPowerExtraIconAmountLabelsChangeSource
    {
        private const int TriggerIntervalTurns = 2;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.OtherworldCrossingPowerIcon);

        public IReadOnlyList<ExtraIconAmountLabelSpec> GetPowerExtraIconAmountLabelSpecs()
        {
            var data = GetInternalData<Data>();
            var turnsLeft = Math.Max(0, data.TurnsUntilTrigger);
            var stacks = (int)Math.Max(0m, Amount);
            return
            [
                ExtraIconAmountLabelSpec.Plain(ExtraIconAmountLabelCorner.BottomLeft, turnsLeft.ToString()),
                ExtraIconAmountLabelSpec.Plain(ExtraIconAmountLabelCorner.BottomRight, stacks.ToString()),
            ];
        }

        event Action? IPowerExtraIconAmountLabelsChangeSource.PowerExtraIconAmountLabelsInvalidated
        {
            add => GetInternalData<Data>().ExtraLabelsInvalidated += value;
            remove => GetInternalData<Data>().ExtraLabelsInvalidated -= value;
        }

        protected override object InitInternalData()
        {
            return new Data();
        }

        public override Task AfterApplied(
            Creature? applier,
            CardModel? cardSource)
        {
            var data = GetInternalData<Data>();
            // Keep timer stable on stack increases: only initialize on first application.
            if (!data.IsInitialized)
            {
                data.TurnsUntilTrigger = TriggerIntervalTurns;
                data.IsInitialized = true;
            }

            data.NotifyExtraLabelsInvalidated();
            return Task.CompletedTask;
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner) return;

            var data = GetInternalData<Data>();
            data.TurnsUntilTrigger--;
            data.NotifyExtraLabelsInvalidated();

            if (data.TurnsUntilTrigger > 0) return;

            data.TurnsUntilTrigger = TriggerIntervalTurns;
            data.NotifyExtraLabelsInvalidated();

            var handCards = PileType.Hand.GetPile(player).Cards.ToList();
            if (handCards.Count == 0) return;

            Flash();

            var copies = Math.Min((int)Math.Max(0m, Amount), handCards.Count);
            if (copies <= 0) return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_POWER_OTHERWORLD_CROSSING_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 0, copies);

            var selected = (await CardSelectCmd.FromHand(choiceContext, player, prefs, null, this)).ToList();
            if (selected.Count == 0) return;

            foreach (var source in selected)
            {
                var clone = source.CreateClone();
                clone.AddKeyword(CardKeyword.Ethereal);
                clone.AddKeyword(CardKeyword.Exhaust);

                var cardInstance = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, player);
                CardCmd.PreviewCardPileAdd(cardInstance);
            }
        }

        public override Task AfterPowerAmountChanged(
            PlayerChoiceContext choiceContext,
            PowerModel power,
            decimal amount,
            Creature? applier,
            CardModel? cardSource)
        {
            if (power != this) return Task.CompletedTask;
            GetInternalData<Data>().NotifyExtraLabelsInvalidated();
            return Task.CompletedTask;
        }

        private class Data
        {
            public bool IsInitialized;
            public int TurnsUntilTrigger = TriggerIntervalTurns;
            public event Action? ExtraLabelsInvalidated;

            public void NotifyExtraLabelsInvalidated()
            {
                ExtraLabelsInvalidated?.Invoke();
            }
        }
    }
}
