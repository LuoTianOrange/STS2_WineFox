using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class ChargePower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ArcaneCapacitorPowerIcon);

        public static ChargePower? Of(Creature? creature) =>
            creature?.Powers.OfType<ChargePower>().FirstOrDefault();

        public static int CountOf(Creature? creature) => (int)(Of(creature)?.Amount ?? 0m);

        public static bool HasConsumer(Creature? creature) =>
            creature?.Powers.OfType<IChargeConsumer>().Any() == true;

        public static async Task Gain(
            PlayerChoiceContext choiceContext,
            Creature creature,
            decimal amount,
            CardModel? sourceCard)
        {
            if (amount <= 0m || !HasConsumer(creature))
                return;

            await PowerCmd.Apply<ChargePower>(choiceContext, creature, amount, creature, sourceCard);
        }

        public static async Task<int> Consume(
            PlayerChoiceContext choiceContext,
            Creature creature,
            int max)
        {
            var power = Of(creature);
            if (power == null || max <= 0)
                return 0;

            var consumed = Math.Min(max, (int)power.Amount);
            if (consumed <= 0)
                return 0;

            if (consumed >= (int)power.Amount)
                await PowerCmd.Remove(power);
            else
                await PowerCmd.ModifyAmount(choiceContext, power, -consumed, creature, null);

            return consumed;
        }
    }
}
