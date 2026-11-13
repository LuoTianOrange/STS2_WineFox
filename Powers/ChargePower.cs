using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Combat.Magic;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public sealed class ChargePower : WineFoxPower
    {
        public const int DetonateThreshold = 10;

        private static bool _detonating;

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
            await TryDetonate(choiceContext, creature, sourceCard);
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

        public static async Task TryDetonate(
            PlayerChoiceContext choiceContext,
            Creature creature,
            CardModel? sourceCard)
        {
            if (_detonating)
                return;

            if (creature.Player is not { } owner)
                return;

            if (CountOf(creature) < DetonateThreshold)
                return;

            _detonating = true;

            try
            {
                while (CountOf(creature) >= DetonateThreshold)
                {
                    await Consume(choiceContext, creature, DetonateThreshold);
                    await MagicWineFoxSpellCmd.CastAll(choiceContext, owner, null, sourceCard, false);
                    await LoadAndTriggerOverload(choiceContext, owner, sourceCard);
                }
            }
            finally
            {
                _detonating = false;
            }
        }

        private static async Task LoadAndTriggerOverload(
            PlayerChoiceContext choiceContext,
            Player owner,
            CardModel? sourceCard)
        {
            if (owner.Creature?.CombatState is not { } combatState)
                return;

            var card = combatState.CreateCard<SpellOverload>(owner);
            var snapshot = new MagicWineFoxSpellSlotSnapshot(card, null, true, 0, card);

            MagicWineFoxSpellCmd.GetPower(owner)?.TryLoad(snapshot);

            card.OverloadCount = Math.Max(1, MagicWineFoxSpellCmd.CountLoadedOverloads(owner));

            await card.CastAsSpell(new MagicWineFoxSpellCastContext(
                choiceContext,
                owner,
                null,
                card,
                sourceCard,
                new MagicWineFoxSpellModifierState()));
        }
    }
}
