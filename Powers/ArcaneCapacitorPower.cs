using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Cards.Spell;
using STS2RitsuLib.Combat.Ui.ExtraCornerAmountLabels;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class ArcaneCapacitorPower : WineFoxPower, IPowerExtraIconAmountLabelsProvider
    {

        protected override IEnumerable<DynamicVar> CanonicalVars => [new("Gain", 2m)];

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ArcaneCapacitorPowerIcon);

        public override int DisplayAmount => Charge;

        [SavedProperty]
        public int Charge
        {
            get;
            set
            {
                AssertMutable();
                field = value;
                InvokeDisplayAmountChanged();
            }
        }

        private decimal Gain =>
            DynamicVars.TryGetValue("Gain", out var gain) ? gain.BaseValue : 2m;

        public IReadOnlyList<ExtraIconAmountLabelSlot> GetPowerExtraIconAmountLabelSlots()
        {
            return [];
        }

        public void Configure(bool upgraded)
        {
            DynamicVars["Gain"].BaseValue = upgraded ? 3m : 2m;
        }

        public Task AddCharge(PlayerChoiceContext choiceContext, decimal amount)
        {
            if (amount <= 0m)
                return Task.CompletedTask;

            Charge += (int)amount;
            return Task.CompletedTask;
        }

        public Task OnSpellLoaded(PlayerChoiceContext choiceContext, CardModel card)
        {
            if (card is not MagicWineFoxSpellCard || card.Type != CardType.Attack)
                return Task.CompletedTask;

            return AddCharge(choiceContext, Gain);
        }

        public override async Task BeforeBlockGained(
            Creature creature,
            decimal amount,
            ValueProp props,
            CardModel? cardSource)
        {
            if (creature != Owner || Charge <= 0)
                return;

            var bonus = Charge;
            Charge = 0;

            await CreatureCmd.GainBlock(Owner, bonus, props, null);
        }
    }
}
