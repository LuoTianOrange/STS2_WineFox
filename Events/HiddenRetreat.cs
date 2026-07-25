using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using STS2_WineFox.Cards.Quest;
using STS2_WineFox.Relics;
using STS2_WineFox.Relics.Event;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Events
{
    [RegisterSharedEvent]
    public sealed class HiddenRetreat : ModEventTemplate
    {
        public override EventAssetProfile AssetProfile => new(InitialPortraitPath: Const.Paths.EventHiddenRetreat);

        public override bool IsAllowed(IRunState runState)
        {
            return false;
        }

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            var offerBell = HasSeekingWindBell()
                ? new EventOption(this, OfferBell, InitialOptionKey("OFFER_BELL"))
                : new EventOption(this, null!, InitialOptionKey("OFFER_BELL_LOCKED"));

            return
            [
                offerBell,
                new(this, Leave, InitialOptionKey("LEAVE")),
            ];
        }

        private bool HasSeekingWindBell()
        {
            return Owner?.Deck.Cards.Any(c => c is SeekingWindBell) == true;
        }

        private IReadOnlyList<EventOption> RelicOptions()
        {
            var options = new List<EventOption>();
            AddRelicOptionIfAllowed<WoundRimeBlade>(options, "WOUND_RIME_BLADE");
            AddRelicOptionIfAllowed<BleedingHeart>(options, "BLEEDING_HEART");
            AddRelicOptionIfAllowed<FlowCore>(options, "FLOW_CORE");
            AddRelicOptionIfAllowed<SilverCercisCrown>(options, "SILVER_CERCIS_CROWN");
            AddRelicOptionIfAllowed<AnchorCore>(options, "ANCHOR_CORE");
            AddRelicOptionIfAllowed<RingofBurningBlood>(options, "RING_OF_BURNING_BLOOD");
            AddRelicOptionIfAllowed<SophisticatedBackpack>(options, "SOPHISTICATED_BACKPACK");

            Shuffle(options);
            return options.Take(3).ToList();
        }

        private void AddRelicOptionIfAllowed<T>(List<EventOption> options, string optionName)
            where T : RelicModel
        {
            ArgumentNullException.ThrowIfNull(Owner);
            if (!ModelDb.Relic<T>().IsAllowed(Owner.RunState))
                return;

            options.Add(CreateRelicOption<T>(optionName));
        }

        private EventOption CreateRelicOption<T>(string optionName)
            where T : RelicModel
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var relic = ModelDb.Relic<T>().ToMutable();
            relic.Owner = Owner;
            return new EventOption(this, () => ChooseRelic<T>(), ModOptionKey("RELICS", optionName),
                CreateRelicHoverTips(relic));
        }

        private static IHoverTip[] CreateRelicHoverTips(RelicModel relic)
        {
            try
            {
                return HoverTipFactory.FromRelic(relic).ToArray();
            }
            catch (InvalidOperationException)
            {
                var relicKey = $"STS2_WINE_FOX_RELIC_{relic.Id.Entry}";
                var description = new LocString("relics", $"{relicKey}.description");
                foreach (var dynamicVar in relic.DynamicVars.Values)
                    description.Add(dynamicVar);

                return
                [
                    new HoverTip(
                        new LocString("relics", $"{relicKey}.title"),
                        description),
                ];
            }
        }

        private void Shuffle<T>(IList<T> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = Rng.NextInt(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private Task OfferBell()
        {
            SetEventState(PageDescription("RELICS"), RelicOptions());
            return Task.CompletedTask;
        }

        private async Task ChooseRelic<T>()
            where T : RelicModel
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var owner = Owner;

            SetEventFinished(PageDescription("OFFER_BELL"));
            await RemoveSeekingWindBell();
            await RelicCmd.Obtain(ModelDb.Relic<T>().ToMutable(), owner);
        }

        private async Task Leave()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var owner = Owner;

            SetEventFinished(PageDescription("LEAVE"));
            await RemoveSeekingWindBell();

            var options = CardCreationOptions
                .ForNonCombatWithUniformOdds(
                    [owner.Character.CardPool],
                    c => c.Rarity == CardRarity.Rare)
                .WithFlags(CardCreationFlags.NoRarityModification | CardCreationFlags.NoCardPoolModifications);

            var rewards = new List<Reward>
            {
                new CardReward(options, 3, owner)
            };
            await RewardsCmd.OfferCustom(owner, rewards);
        }

        private async Task RemoveSeekingWindBell()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var owner = Owner;

            var cards = owner.Deck.Cards
                .Where(c => c is SeekingWindBell)
                .ToList();

            foreach (var card in cards)
            {
                PlayerCmd.CompleteQuest(card);
                await CardPileCmd.RemoveFromDeck(card);
            }
        }
    }
}
