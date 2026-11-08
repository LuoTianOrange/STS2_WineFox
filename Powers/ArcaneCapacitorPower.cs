using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class ArcaneCapacitorPower : WineFoxPower, IChargeConsumer
    {
        private const int ConsumeCapPerCopy = 6;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Gain", 2m),
            new("Cap", 6m)
        ];

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ArcaneCapacitorPowerIcon);

        [SavedProperty]
        public int PerChargeBlock
        {
            get;
            set
            {
                AssertMutable();
                field = value;
            }
        }

        private int ConsumeCap => Math.Max(ConsumeCapPerCopy, (int)Amount * ConsumeCapPerCopy);

        private decimal BlockPerCharge =>
            PerChargeBlock > 0 ? PerChargeBlock : DynamicVars["Gain"].BaseValue;

        public void Configure(bool upgraded)
        {
            PerChargeBlock += upgraded ? 3 : 2;
            SyncDisplay();
        }

        private void SyncDisplay()
        {
            if (PerChargeBlock > 0)
                DynamicVars["Gain"].BaseValue = PerChargeBlock;

            DynamicVars["Cap"].BaseValue = ConsumeCap;
        }

        public Task OnSpellLoaded(PlayerChoiceContext choiceContext, CardModel card)
        {
            if (Owner is not { } owner)
                return Task.CompletedTask;

            return ChargePower.Gain(choiceContext, owner, 1m, card);
        }

        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (Owner is not { } owner || side != owner.Side)
                return;

            SyncDisplay();

            var consumed = await ChargePower.Consume(choiceContext, owner, ConsumeCap);
            if (consumed <= 0)
                return;

            await CreatureCmd.GainBlock(owner, consumed * BlockPerCharge, ValueProp.Move, null);
        }
    }
}
