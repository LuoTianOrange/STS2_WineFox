using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace STS2_WineFox.Powers
{
    public abstract class PotionDeathMarkPower : WineFoxPower
    {
        private bool _resolved;
        protected Player? SourcePlayer { get; private set; }

        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Single;

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            SourcePlayer = applier?.Player;
            return Task.CompletedTask;
        }

        public override bool ShouldStopCombatFromEnding()
        {
            return !_resolved && Owner.IsDead;
        }

        public sealed override async Task AfterDeath(
            PlayerChoiceContext choiceContext,
            Creature creature,
            bool wasRemovalPrevented,
            float deathAnimLength)
        {
            if (_resolved || wasRemovalPrevented || creature != Owner)
                return;

            _resolved = true;
            Flash();
            await OnMarkedOwnerDied(choiceContext, deathAnimLength);
        }

        protected abstract Task OnMarkedOwnerDied(PlayerChoiceContext choiceContext, float deathAnimLength);

        protected void AddCardReward(CardRarity rarity)
        {
            if (SourcePlayer == null || CombatState.RunState.CurrentRoom is not CombatRoom room)
                return;

            var options = CardCreationOptions
                .ForNonCombatWithUniformOdds(
                    [SourcePlayer.Character.CardPool],
                    card => card.Rarity == rarity)
                .WithFlags(CardCreationFlags.NoRarityModification);

            room.AddExtraReward(SourcePlayer, new CardReward(options, 3, SourcePlayer));
        }
    }
}
