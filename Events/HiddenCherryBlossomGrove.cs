using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using STS2_WineFox.Cards.Quest;
using STS2_WineFox.Settings;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Events
{
    [RegisterActEvent(typeof(Overgrowth))]
    [RegisterActEvent(typeof(Underdocks))]
    public sealed class HiddenCherryBlossomGrove : ModEventTemplate
    {
        public override bool IsShared => true;
        public override EventAssetProfile AssetProfile => new(InitialPortraitPath: Const.Paths.EventHiddenCherryBlossomGrove);

        public override bool IsAllowed(IRunState runState)
        {
            return WineFoxRuntimeSettings.EventsEnabled
                   && runState.Players.All(p => p.Deck.Cards.All(c => c is not SeekingWindBell))
                   && base.IsAllowed(runState);
        }

        protected override IReadOnlyList<EventOption> GenerateInitialOptions()
        {
            return
            [
                new(this, TrustIntuition, InitialOptionKey("TRUST_INTUITION"),
                    HoverTipFactory.FromCardWithCardHoverTips<SeekingWindBell>()),
                new(this, LeaveCarefully, InitialOptionKey("LEAVE_CAREFULLY")),
            ];
        }

        private async Task TrustIntuition()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var owner = Owner;

            SetEventFinished(PageDescription("TRUST_INTUITION"));
            var rewards = new List<Reward>
            {
                new SpecialCardReward(owner.RunState.CreateCard<SeekingWindBell>(owner), owner)
            };
            await RewardsCmd.OfferCustom(owner, rewards);
        }

        private async Task LeaveCarefully()
        {
            ArgumentNullException.ThrowIfNull(Owner);
            var owner = Owner;

            await PlayerCmd.LoseGold(owner.Gold, owner, GoldLossType.Lost);

            var rewards = new List<Reward>
            {
                new CardRemovalReward(owner)
            };
            await RewardsCmd.OfferCustom(owner, rewards);

            SetEventFinished(PageDescription("LEAVE_CAREFULLY"));
        }
    }
}
