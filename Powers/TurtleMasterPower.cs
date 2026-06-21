using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class TurtleMasterPower : WineFoxPower, IPowerExtraIconAmountLabelsProvider
    {
        public const int Duration = 2;
        private const decimal ReductionPerStack = 0.2m;

        protected override IEnumerable<DynamicVar> CanonicalVars => [new ReductionVar(), new RemainingTurnsVar()];

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        private int _remainingTurns = Duration;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ResistancePowerIcon);
        public override int DisplayAmount => RemainingTurns;

        [SavedProperty]
        public int RemainingTurns
        {
            get => _remainingTurns;
            set
            {
                AssertMutable();
                _remainingTurns = value;
                InvokeDisplayAmountChanged();
            }
        }

        public IReadOnlyList<ExtraIconAmountLabelSlot> GetPowerExtraIconAmountLabelSlots()
        {
            return [ExtraIconAmountLabelSlot.At(ExtraIconAmountLabelCorner.BottomLeft, Amount.ToString())];
        }

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            RemainingTurns = Duration;
            InvokeDisplayAmountChanged();
            return Task.CompletedTask;
        }

        public override decimal ModifyHpLostAfterOsty(
            Creature target,
            decimal amount,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (target != Owner)
                return amount;

            var multiplier = Math.Max(0m, 1m - Amount * ReductionPerStack);
            return Math.Floor(amount * multiplier);
        }

        public override decimal ModifyBlockMultiplicative(
            Creature target,
            decimal block,
            ValueProp props,
            CardModel? cardSource,
            CardPlay? cardPlay)
        {
            return target == Owner ? 0m : 1m;
        }

        public override Task AfterModifyingHpLostAfterOsty()
        {
            Flash();
            return Task.CompletedTask;
        }

        protected override async Task OnAfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player.Creature != Owner)
                return;

            RemainingTurns--;
            InvokeDisplayAmountChanged();

            if (RemainingTurns <= 0)
                await PowerCmd.Remove(this);
        }

        private sealed class ReductionVar : TurtleMasterVar
        {
            public ReductionVar()
                : base("Reduction")
            {
            }

            protected override decimal Calculate()
            {
                return Power?.Amount * 20m ?? 0m;
            }
        }

        private sealed class RemainingTurnsVar : TurtleMasterVar
        {
            public RemainingTurnsVar()
                : base("RemainingTurns")
            {
            }

            protected override decimal Calculate()
            {
                return Power?.RemainingTurns ?? Duration;
            }
        }

        private abstract class TurtleMasterVar(string name) : DynamicVar(name, 0m)
        {
            protected TurtleMasterPower? Power => _owner as TurtleMasterPower;

            public override void SetOwner(AbstractModel owner)
            {
                base.SetOwner(owner);
                UpdateValues();
            }

            public override string ToString()
            {
                return Calculate().ToString();
            }

            protected override decimal GetBaseValueForIConvertible()
            {
                return Calculate();
            }

            protected abstract decimal Calculate();

            private void UpdateValues()
            {
                var value = Calculate();
                BaseValue = value;
                PreviewValue = value;
                EnchantedValue = value;
            }
        }
    }
}
