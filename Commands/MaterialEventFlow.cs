using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Commands
{
    public sealed record MaterialDelta(Type MaterialType, decimal Amount);

    public enum MaterialChangeKind
    {
        Gain,
        Consume,
    }

    public sealed class MaterialGainEvent
    {
        public required Creature Creature { get; init; }
        public required CardModel? SourceCard { get; init; }
        public required IReadOnlyList<MaterialDelta> Deltas { get; init; }
        public required decimal TotalAmount { get; init; }
        public required bool AppliedStressMultiplier { get; init; }
    }

    public sealed class MaterialConsumeEvent
    {
        public required Creature Creature { get; init; }
        public required CardModel? SourceCard { get; init; }
        public required IReadOnlyList<MaterialDelta> Deltas { get; init; }
        public required decimal TotalAmount { get; init; }
    }

    public sealed class MaterialResolvedEvent
    {
        public required Creature Creature { get; init; }
        public required CardModel? SourceCard { get; init; }
        public required IReadOnlyList<MaterialDelta> Deltas { get; init; }
        public required decimal TotalAmount { get; init; }
        public required MaterialChangeKind Kind { get; init; }
        public required bool AppliedStressMultiplier { get; init; }
    }

    public interface IMaterialEventListener
    {
        Task BeforeMaterialGain(MaterialGainEvent evt);
        Task AfterMaterialGain(MaterialGainEvent evt);
        Task BeforeMaterialConsume(MaterialConsumeEvent evt);
        Task AfterMaterialConsume(MaterialConsumeEvent evt);
        Task AfterMaterialResolved(MaterialResolvedEvent evt);
    }

    public static class MaterialEventFlow
    {
        public static async Task DispatchBeforeGain(MaterialGainEvent evt)
        {
            if (evt.TotalAmount <= 0m)
                return;

            var listeners = evt.Creature.Powers
                .OfType<IMaterialEventListener>()
                .ToList();

            foreach (var listener in listeners)
                await listener.BeforeMaterialGain(evt);
        }

        public static async Task DispatchAfterGain(MaterialGainEvent evt)
        {
            if (evt.TotalAmount <= 0m)
                return;

            var listeners = evt.Creature.Powers
                .OfType<IMaterialEventListener>()
                .ToList();

            foreach (var listener in listeners)
                await listener.AfterMaterialGain(evt);
        }

        public static async Task DispatchBeforeConsume(MaterialConsumeEvent evt)
        {
            if (evt.TotalAmount <= 0m)
                return;

            var listeners = evt.Creature.Powers
                .OfType<IMaterialEventListener>()
                .ToList();

            foreach (var listener in listeners)
                await listener.BeforeMaterialConsume(evt);
        }

        public static async Task DispatchAfterConsume(MaterialConsumeEvent evt)
        {
            if (evt.TotalAmount <= 0m)
                return;

            var listeners = evt.Creature.Powers
                .OfType<IMaterialEventListener>()
                .ToList();

            foreach (var listener in listeners)
                await listener.AfterMaterialConsume(evt);
        }

        public static async Task DispatchAfterResolved(MaterialResolvedEvent evt)
        {
            if (evt.TotalAmount <= 0m)
                return;

            var listeners = evt.Creature.Powers
                .OfType<IMaterialEventListener>()
                .ToList();

            foreach (var listener in listeners)
                await listener.AfterMaterialResolved(evt);

            CraftCmd.RecordMaterialResolved(evt);
        }
    }
}
