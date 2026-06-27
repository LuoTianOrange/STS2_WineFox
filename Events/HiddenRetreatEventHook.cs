using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;
using STS2_WineFox.Cards.Quest;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;

namespace STS2_WineFox.Events
{
    [RegisterSingleton]
    public sealed class HiddenRetreatEventHook : HookedSingletonModel
    {
        public HiddenRetreatEventHook()
            : base(HookType.Run)
        {
        }

        public override IReadOnlySet<RoomType> ModifyUnknownMapPointRoomTypes(IReadOnlySet<RoomType> roomTypes)
        {
            return ShouldForceHiddenRetreat() ? new HashSet<RoomType> { RoomType.Event } : roomTypes;
        }

        public override EventModel ModifyNextEvent(EventModel currentEvent)
        {
            return ShouldForceHiddenRetreat() ? ModelDb.Event<HiddenRetreat>() : currentEvent;
        }

        private bool ShouldForceHiddenRetreat()
        {
            var runState = CurrentRunState;
            if (runState == null)
                return false;

            return runState.Players
                .Where(p => p.IsActiveForHooks)
                .SelectMany(p => p.Deck.Cards)
                .OfType<SeekingWindBell>()
                .Any(bell => bell.TargetActIndex <= runState.CurrentActIndex);
        }
    }
}
