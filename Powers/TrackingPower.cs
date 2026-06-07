using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using System.Globalization;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     Tracking power applied to the player by IntermittentChanting.
    ///     Whenever the owner causes an enemy to lose HP (Unblockable damage),
    ///     the target gains 1 block and the owner gains Amount block.
    /// </summary>
    [RegisterPower]
    public class TrackingPower : WineFoxPower
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [new EnemyBlockVar()];

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.TrackingPowerIcon);

        public override async Task AfterDamageGiven(
            PlayerChoiceContext choiceContext,
            Creature? dealer,
            DamageResult result,
            ValueProp props,
            Creature target,
            CardModel? cardSource)
        {
            if (dealer != Owner) return;
            if (target.Side == Owner.Side) return;
            if (result.UnblockedDamage <= 0) return;

            Flash();
            var targetBlock = GetEnemyBlockAmount();
            await CreatureCmd.GainBlock(target, targetBlock, ValueProp.Unpowered, null);
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        }

        protected override object InitInternalData()
        {
            return new Data();
        }

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            var data = GetInternalData<Data>();
            if (!data.IsInitialized)
            {
                data.ApplicationCount = 1;
                data.IsInitialized = true;
            }

            data.LastKnownAmount = Math.Max(data.LastKnownAmount, Amount);
            return Task.CompletedTask;
        }

        public override Task AfterPowerAmountChanged(
            PowerModel power,
            decimal amount,
            Creature? applier,
            CardModel? cardSource)
        {
            if (power != this) return Task.CompletedTask;

            var data = GetInternalData<Data>();
            if (!data.IsInitialized)
            {
                data.ApplicationCount = Amount > 0m ? 1 : 0;
                data.IsInitialized = true;
            }
            else if (amount > 0m && Amount > data.LastKnownAmount)
            {
                data.ApplicationCount++;
            }

            data.LastKnownAmount = Amount;
            return Task.CompletedTask;
        }

        private decimal GetEnemyBlockAmount()
        {
            var applications = GetInternalData<Data>().ApplicationCount;
            if (applications > 0)
                return applications;

            return Math.Max(1m, Math.Floor(Amount / 2m));
        }

        private sealed class EnemyBlockVar : DynamicVar
        {
            public EnemyBlockVar() : base("EnemyBlock", 1m)
            {
            }

            public override string ToString()
            {
                return Calculate().ToString(CultureInfo.InvariantCulture);
            }

            protected override decimal GetBaseValueForIConvertible()
            {
                return Calculate();
            }

            private decimal Calculate()
            {
                return _owner is TrackingPower power
                    ? power.GetEnemyBlockAmount()
                    : 1m;
            }
        }

        private class Data
        {
            public bool IsInitialized;
            public decimal LastKnownAmount;
            public int ApplicationCount;
        }
    }
}
