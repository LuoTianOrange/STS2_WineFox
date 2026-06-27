using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics.Event
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class FlowCore : WineFoxRelic
    {
        private const int TurnInterval = 3;
        private const int MaxHpLossPerTurn = 15;

        private int _hpLostThisTurn;

        public override RelicRarity Rarity => RelicRarity.Event;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.FlowCoreRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar("TurnInterval", TurnInterval),
            new EnergyVar(1),
            new HealVar(1m),
            new DynamicVar("MaxHpLoss", MaxHpLossPerTurn),
        ];

        [SavedProperty]
        public int HpLostThisTurn
        {
            get => _hpLostThisTurn;
            set
            {
                AssertMutable();
                _hpLostThisTurn = value;
            }
        }

        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != Owner)
                return;

            HpLostThisTurn = 0;
            if (player.PlayerCombatState?.TurnNumber % TurnInterval != 0)
                return;

            Flash();
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        }

        public override decimal ModifyHpLostAfterOsty(
            Creature target,
            decimal amount,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (target != Owner.Creature)
                return amount;

            var remaining = MaxHpLossPerTurn - HpLostThisTurn;
            return Math.Max(0m, Math.Min(amount, remaining));
        }

        public override Task AfterDamageReceived(
            PlayerChoiceContext choiceContext,
            Creature target,
            DamageResult result,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (target == Owner.Creature && result.UnblockedDamage > 0)
                HpLostThisTurn += result.UnblockedDamage;

            return Task.CompletedTask;
        }
    }
}
