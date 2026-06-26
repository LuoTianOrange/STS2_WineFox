using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    [RegisterRelic(typeof(WineFoxSharedRelicPool))]
    public sealed class Elytra : WineFoxRelic
    {
        private const string RoomsKey = "Rooms";
        private const int MaxCharges = 3;

        private int _timesUsed;

        public override RelicRarity Rarity => RelicRarity.Rare;
        public override RelicAssetProfile AssetProfile =>
            Icons(IsUsedUp ? Const.Paths.BrokenElytraRelicIcon : Const.Paths.ElytraRelicIcon);
        public override bool IsUsedUp => TimesUsed >= MaxCharges;
        public override bool ShowCounter => !IsUsedUp;
        public override int DisplayAmount => MaxCharges - TimesUsed;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DynamicVar(RoomsKey, MaxCharges)];

        [SavedProperty]
        public int TimesUsed
        {
            get => _timesUsed;
            set
            {
                AssertMutable();
                _timesUsed = value;
                DynamicVars[RoomsKey].BaseValue = MaxCharges - _timesUsed;
                InvokeDisplayAmountChanged();
                RelicIconChanged();
                Status = IsUsedUp ? RelicStatus.Disabled : RelicStatus.Normal;
            }
        }

        public override bool IsAllowed(IRunState runState)
        {
            return runState.Players.Count == 1;
        }

        public override bool ShouldAllowFreeTravel()
        {
            return !IsUsedUp;
        }

        public override Task AfterRoomEntered(AbstractRoom room)
        {
            if (IsUsedUp)
                return Task.CompletedTask;

            if (Owner.RunState.CurrentRoomCount > 1)
                return Task.CompletedTask;

            if (Owner.RunState is not RunState runState)
                return Task.CompletedTask;

            if (runState.VisitedMapCoords.Count <= 1)
                return Task.CompletedTask;

            IReadOnlyList<MapCoord> visitedMapCoords = runState.VisitedMapCoords;
            var previousCoord = visitedMapCoords[visitedMapCoords.Count - 2];
            var previousPoint = runState.Map.GetPoint(previousCoord);
            if (previousPoint == null)
                return Task.CompletedTask;

            var currentPoint = Owner.RunState.CurrentMapPoint;
            if (currentPoint == null)
                return Task.CompletedTask;

            if (previousPoint.Children.Contains(currentPoint))
                return Task.CompletedTask;

            TimesUsed++;
            Flash();
            return Task.CompletedTask;
        }

        public override Task AfterCombatEnd(CombatRoom room)
        {
            if (room.RoomType != RoomType.Elite)
                return Task.CompletedTask;

            if (TimesUsed <= 0)
                return Task.CompletedTask;

            TimesUsed--;
            Flash();
            return Task.CompletedTask;
        }
    }
}
