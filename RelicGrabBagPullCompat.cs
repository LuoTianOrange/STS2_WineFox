using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace STS2_WineFox
{
    internal static class RelicGrabBagPullCompat
    {
        public static readonly Func<RelicModel, bool> PassAllRelics = static _ => true;

        private static readonly PullFromFrontDelegate? NativePull = CreateNativePull();

        private static readonly FieldInfo? DequesField =
            typeof(RelicGrabBag).GetField("_deques", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo? RefreshAllowedField =
            typeof(RelicGrabBag).GetField("_refreshAllowed", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo? OriginalRelicsField =
            typeof(RelicGrabBag).GetField("_originalRelics", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly FieldInfo? MpFallbackField =
            typeof(RelicGrabBag).GetField("_mpFallbackDequeue", BindingFlags.Instance | BindingFlags.NonPublic);

        private static PullFromFrontDelegate? CreateNativePull()
        {
            var m = typeof(RelicGrabBag).GetMethod(
                nameof(RelicGrabBag.PullFromFront),
                [typeof(RelicRarity), typeof(Func<RelicModel, bool>), typeof(IRunState)]);
            if (m == null)
                return null;
            try
            {
                return (PullFromFrontDelegate)Delegate.CreateDelegate(typeof(PullFromFrontDelegate), m);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static RelicModel? PullFromFront(
            RelicGrabBag bag,
            RelicRarity rarity,
            Func<RelicModel, bool> filter,
            IRunState runState)
        {
            return NativePull != null
                ? NativePull(bag, rarity, filter, runState)
                : PullFromFrontReflected(bag, rarity, filter, runState);
        }

        private static RelicModel? PullFromFrontReflected(
            RelicGrabBag bag,
            RelicRarity rarity,
            Func<RelicModel, bool> filter,
            IRunState runState)
        {
            if (DequesField == null || RefreshAllowedField == null || OriginalRelicsField == null ||
                MpFallbackField == null)
                return FilterTrueFallback(bag, rarity, filter, runState);

            var deques = (Dictionary<RelicRarity, List<RelicModel>>)DequesField.GetValue(bag)!;
            var refreshAllowed = (bool)RefreshAllowedField.GetValue(bag)!;
            var originalRelics = (List<RelicModel>?)OriginalRelicsField.GetValue(bag);
            var mpFallback = (List<RelicModel>)MpFallbackField.GetValue(bag)!;

            RemoveDisallowedRelicsFromDeques(deques, mpFallback, runState);

            var availableDeque = GetAvailableDeque(deques, mpFallback, originalRelics, refreshAllowed, rarity, runState,
                filter);
            if (availableDeque == null || availableDeque.Count == 0)
                return null;

            for (var i = 0; i < availableDeque.Count; i++)
            {
                var relicModel = availableDeque[i];
                if (!filter(relicModel)) continue;
                availableDeque.RemoveAt(i);
                return relicModel;
            }

            return null;
        }

        private static RelicModel? FilterTrueFallback(
            RelicGrabBag bag,
            RelicRarity rarity,
            Func<RelicModel, bool> filter,
            IRunState runState)
        {
            return ReferenceEquals(filter, PassAllRelics) ? bag.PullFromFront(rarity, runState) : null;
        }

        private static void RemoveDisallowedRelicsFromDeques(
            Dictionary<RelicRarity, List<RelicModel>> deques,
            List<RelicModel> mpFallbackDequeue,
            IRunState runState)
        {
            foreach (var value in deques.Select(deque => deque.Value))
                for (var i = 0; i < value.Count; i++)
                    if (!value[i].IsAllowed(runState))
                    {
                        value.RemoveAt(i);
                        i--;
                    }

            for (var j = 0; j < mpFallbackDequeue.Count; j++)
                if (!mpFallbackDequeue[j].IsAllowed(runState))
                {
                    mpFallbackDequeue.RemoveAt(j);
                    j--;
                }
        }

        private static List<RelicModel>? GetAvailableDeque(
            Dictionary<RelicRarity, List<RelicModel>> deques,
            List<RelicModel> mpFallbackDequeue,
            List<RelicModel>? originalRelics,
            bool refreshAllowed,
            RelicRarity rarity,
            IRunState runState,
            Func<RelicModel, bool> filter)
        {
            RemoveDisallowedRelicsFromDeques(deques, mpFallbackDequeue, runState);
            var list = GetDeque(deques, rarity);
            if (list.Count == 0 && refreshAllowed)
            {
                RefreshRarity(deques, originalRelics, rarity);
                RemoveDisallowedRelicsFromDeques(deques, mpFallbackDequeue, runState);
            }

            while (list != null && !DequeHasAnyRelics(list, filter))
            {
                rarity = rarity switch
                {
                    RelicRarity.Shop => RelicRarity.Common,
                    RelicRarity.Common => RelicRarity.Uncommon,
                    RelicRarity.Uncommon => RelicRarity.Rare,
                    _ => RelicRarity.None,
                };
                list = rarity == RelicRarity.None ? null : GetDeque(deques, rarity);
            }

            if (list == null && DequeHasAnyRelics(mpFallbackDequeue, filter))
                list = mpFallbackDequeue;

            return list;
        }

        private static bool DequeHasAnyRelics(List<RelicModel> deque, Func<RelicModel, bool> filter)
        {
            return deque.Any(filter.Invoke);
        }

        private static List<RelicModel> GetDeque(Dictionary<RelicRarity, List<RelicModel>> deques, RelicRarity rarity)
        {
            return deques.TryGetValue(rarity, out var value) ? value : new();
        }

        private static void RefreshRarity(
            Dictionary<RelicRarity, List<RelicModel>> deques,
            List<RelicModel>? originalRelics,
            RelicRarity rarity)
        {
            if (originalRelics == null)
                throw new InvalidOperationException("Tried to refresh relics but original list is null");

            foreach (var originalRelic in originalRelics.Where(originalRelic => originalRelic.Rarity == rarity))
            {
                if (!deques.TryGetValue(originalRelic.Rarity, out var value))
                {
                    value = new();
                    deques[originalRelic.Rarity] = value;
                }

                value.Add(originalRelic);
            }
        }

        private delegate RelicModel? PullFromFrontDelegate(
            RelicGrabBag bag,
            RelicRarity rarity,
            Func<RelicModel, bool> filter,
            IRunState runState);
    }
}
