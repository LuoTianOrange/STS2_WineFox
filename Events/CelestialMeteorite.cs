using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Cards.Event;
using STS2_WineFox.Settings;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Events
{
    [RegisterActEvent(typeof(Overgrowth))]
    [RegisterActEvent(typeof(Underdocks))]
    public sealed class CelestialMeteorite : ModEventTemplate
    {
        public override EventAssetProfile AssetProfile => new(InitialPortraitPath: Const.Paths.EventCelestialMeteorite);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DynamicVar("HpLossPercent", 10m),
            new DynamicVar("MinGold", 40m),
            new DynamicVar("MaxGold", 60m),
        ];

        public override bool IsAllowed(IRunState runState)
        {
            return WineFoxRuntimeSettings.EventsEnabled
                   && base.IsAllowed(runState);
        }

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            return
            [
                new(this, OpenMeteorite, InitialOptionKey("OPEN_METEORITE"),
                    HoverTipFactory.FromCardWithCardHoverTips<CraftingStorage>()),
                new(this, TakeSample, InitialOptionKey("TAKE_SAMPLE")),
                new(this, Leave, InitialOptionKey("LEAVE")),
            ];
        }

        private async Task OpenMeteorite()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var owner = Owner;

            await LosePercentHp();
            SetEventFinished(PageDescription("OPEN_METEORITE"));

            var rewards = new List<Reward>
            {
                new SpecialCardReward(owner.RunState.CreateCard<CraftingStorage>(owner), owner)
            };
            await RewardsCmd.OfferCustom(owner, rewards);
        }

        private async Task TakeSample()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var owner = Owner;

            await LosePercentHp();
            SetEventFinished(PageDescription("TAKE_SAMPLE"));

            var options = CardCreationOptions
                .ForNonCombatWithUniformOdds(
                    [ModelDb.CardPool<ColorlessCardPool>()],
                    c => c.Rarity is not CardRarity.Basic and not CardRarity.Token)
                .WithFlags(CardCreationFlags.NoRarityModification | CardCreationFlags.NoCardPoolModifications);

            var rewards = new List<Reward>
            {
                new CardReward(options, 3, owner)
            };
            await RewardsCmd.OfferCustom(owner, rewards);
        }

        private async Task Leave()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var owner = Owner;
            var gold = owner.PlayerRng.Rewards.NextInt(
                DynamicVars["MinGold"].IntValue,
                DynamicVars["MaxGold"].IntValue + 1);

            await PlayerCmd.GainGold(gold, owner);
            SetEventFinished(PageDescription("LEAVE"));
        }

        private async Task LosePercentHp()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var hpLoss = Math.Ceiling(Owner.Creature.MaxHp * DynamicVars["HpLossPercent"].BaseValue / 100m);
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, hpLoss,
                ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        }
    }
}
