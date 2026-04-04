using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.FreePlay;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Commands
{
    public static class MaterialCmd
    {
        private static readonly AttachedState<CardModel, MaterialConsumeSeriesState> MaterialConsumeSeriesStates =
            new(() => new());

        /// <summary>
        ///     Resolves material amount for a delayed effect granted by a card (e.g. Plant): applies stress
        ///     doubling and consumption now, like <see cref="DodgeAndRoll" /> locking block for next turn.
        ///     The returned value should be stored on a power and granted later without going through
        ///     <see cref="MaterialCmd" /> again.
        /// </summary>
        public static async Task<decimal> ResolveCardMaterialAmount(
            Creature creature,
            CardModel card,
            decimal baseAmount,
            bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(creature);
            ArgumentNullException.ThrowIfNull(card);

            if (baseAmount <= 0m)
                return 0m;

            var stress = applyStress && await TryTriggerStressPower(creature) ? 2m : 1m;
            return baseAmount * stress;
        }

        public static async Task GainMaterial<T>(CardModel card, decimal amount, bool applyStress = true)
            where T : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            if (amount <= 0m)
                return;

            await CommitCardMaterialGains(card, [(typeof(T), amount)], applyStress);
        }

        public static async Task GainMaterial<T>(Creature creature, decimal amount, CardModel? sourceCard = null,
            bool applyStress = true)
            where T : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(creature);

            if (amount <= 0m)
                return;

            await CommitCardMaterialGains(creature, sourceCard, [(typeof(T), amount)], applyStress);
        }

        public static async Task GainMaterials<TFirst, TSecond>(CardModel card, decimal firstAmount,
            decimal secondAmount, bool applyStress = true)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);

            var gains = new List<(Type Type, decimal Amount)>(2);
            if (firstAmount > 0m)
                gains.Add((typeof(TFirst), firstAmount));
            if (secondAmount > 0m)
                gains.Add((typeof(TSecond), secondAmount));

            if (gains.Count == 0)
                return;

            await CommitCardMaterialGains(card, gains, applyStress);
        }

        public static async Task GainMaterials<TFirst, TSecond>(Creature creature, decimal firstAmount,
            decimal secondAmount, CardModel? sourceCard = null, bool applyStress = true)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(creature);

            var gains = new List<(Type Type, decimal Amount)>(2);
            if (firstAmount > 0m)
                gains.Add((typeof(TFirst), firstAmount));
            if (secondAmount > 0m)
                gains.Add((typeof(TSecond), secondAmount));

            if (gains.Count == 0)
                return;

            await CommitCardMaterialGains(creature, sourceCard, gains, applyStress);
        }

        public static async Task GainMaterials(CardModel card, IEnumerable<(Type Type, decimal Amount)> materials,
            bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(card);
            ArgumentNullException.ThrowIfNull(materials);

            var gains = materials.Where(m => m.Amount > 0m).ToList();
            if (gains.Count == 0)
                return;

            await CommitCardMaterialGains(card, gains, applyStress);
        }

        public static async Task GainMaterials(Creature creature, IEnumerable<(Type Type, decimal Amount)> materials,
            CardModel? sourceCard = null, bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(creature);
            ArgumentNullException.ThrowIfNull(materials);

            var gains = materials.Where(m => m.Amount > 0m).ToList();
            if (gains.Count == 0)
                return;

            await CommitCardMaterialGains(creature, sourceCard, gains, applyStress);
        }

        /// <summary>
        ///     From a played card: one stress check for the whole batch, then one iron pickaxe check.
        /// </summary>
        public static async Task GainAllMaterials(CardModel card, decimal amountPerType, bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(card);

            if (amountPerType <= 0m)
                return;

            var gains = MaterialPowerRegistry.RegisteredMaterialTypes
                .Select(t => (t, amountPerType))
                .ToList();

            await CommitCardMaterialGains(card, gains, applyStress);
        }

        public static async Task GainAllMaterials(Creature creature, decimal amountPerType, bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(creature);

            if (amountPerType <= 0m)
                return;

            var appliedStressMultiplier = false;
            if (applyStress && await TryTriggerStressPower(creature))
            {
                amountPerType *= 2m;
                appliedStressMultiplier = true;
            }

            var deltas = MaterialPowerRegistry.RegisteredMaterialTypes
                .Select(type => new MaterialDelta(type, amountPerType))
                .ToList();
            var gainEvent = new MaterialGainEvent
            {
                Creature = creature,
                SourceCard = null,
                Deltas = deltas,
                TotalAmount = deltas.Sum(d => d.Amount),
                AppliedStressMultiplier = appliedStressMultiplier,
            };
            await MaterialEventFlow.DispatchBeforeGain(gainEvent);
            await MaterialPowerRegistry.ApplyAll(creature, amountPerType, null);
            await MaterialEventFlow.DispatchAfterGain(gainEvent);
            await MaterialEventFlow.DispatchAfterResolved(new()
            {
                Creature = gainEvent.Creature,
                SourceCard = gainEvent.SourceCard,
                Deltas = gainEvent.Deltas,
                TotalAmount = gainEvent.TotalAmount,
                Kind = MaterialChangeKind.Gain,
                AppliedStressMultiplier = gainEvent.AppliedStressMultiplier,
            });
        }

        public static async Task<decimal> ConsumeAllMaterialsForSeries(CardModel card, CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(card);
            ArgumentNullException.ThrowIfNull(play);

            var owner = card.Owner?.Creature ??
                        throw new InvalidOperationException("Material source has no owner creature.");
            var state = EnsureSeriesState(card, play);

            if (play.IsFirstInSeries)
            {
                var entries = MaterialPowerRegistry.RegisteredMaterialTypes
                    .Select(type =>
                    {
                        var power = owner.Powers.FirstOrDefault(p => p.GetType() == type);
                        var amount = power?.Amount ?? 0m;
                        return (Type: type, Power: power, Amount: amount);
                    })
                    .Where(entry => entry.Amount > 0m)
                    .ToList();
                var totalAmount = entries.Sum(entry => entry.Amount);

                state.AllMaterialsAmount = totalAmount;
                state.HasAllMaterialsSnapshot = true;

                if (state.IsFreePlay || totalAmount <= 0m) return totalAmount;
                {
                    var consumeEvent = new MaterialConsumeEvent
                    {
                        Creature = owner,
                        SourceCard = card,
                        Deltas = entries.Select(entry => new MaterialDelta(entry.Type, entry.Amount)).ToList(),
                        TotalAmount = totalAmount,
                    };
                    await MaterialEventFlow.DispatchBeforeConsume(consumeEvent);
                    foreach (var entry in entries)
                        if (entry.Power != null)
                            await PowerCmd.ModifyAmount(entry.Power, -entry.Amount, null, card);
                    await MaterialEventFlow.DispatchAfterConsume(consumeEvent);
                    await MaterialEventFlow.DispatchAfterResolved(new()
                    {
                        Creature = consumeEvent.Creature,
                        SourceCard = consumeEvent.SourceCard,
                        Deltas = consumeEvent.Deltas,
                        TotalAmount = consumeEvent.TotalAmount,
                        Kind = MaterialChangeKind.Consume,
                        AppliedStressMultiplier = false,
                    });
                    CraftCmd.RecordMaterialConsume(owner);
                }

                return totalAmount;
            }

            if (state.IsFreePlay)
                return GetTotalMaterials(owner);

            return state.HasAllMaterialsSnapshot ? state.AllMaterialsAmount : 0m;
        }

        public static async Task<decimal> ConsumeAllMaterialOfTypeForSeries<T>(CardModel card, CardPlay play)
            where T : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);
            ArgumentNullException.ThrowIfNull(play);

            var owner = card.Owner?.Creature ??
                        throw new InvalidOperationException("Material source has no owner creature.");
            var state = EnsureSeriesState(card, play);
            var materialType = typeof(T);

            if (play.IsFirstInSeries)
            {
                var power = owner.Powers.OfType<T>().FirstOrDefault();
                var amount = power?.Amount ?? 0m;
                state.MaterialTypeAmounts[materialType] = amount;

                if (state.IsFreePlay || amount <= 0m || power == null) return amount;
                var consumeEvent = new MaterialConsumeEvent
                {
                    Creature = owner,
                    SourceCard = card,
                    Deltas = [new(materialType, amount)],
                    TotalAmount = amount,
                };
                await MaterialEventFlow.DispatchBeforeConsume(consumeEvent);
                await PowerCmd.ModifyAmount(power, -amount, null, card);
                await MaterialEventFlow.DispatchAfterConsume(consumeEvent);
                await MaterialEventFlow.DispatchAfterResolved(new()
                {
                    Creature = consumeEvent.Creature,
                    SourceCard = consumeEvent.SourceCard,
                    Deltas = consumeEvent.Deltas,
                    TotalAmount = consumeEvent.TotalAmount,
                    Kind = MaterialChangeKind.Consume,
                    AppliedStressMultiplier = false,
                });
                CraftCmd.RecordMaterialConsume(owner);

                return amount;
            }

            if (state.IsFreePlay)
                return owner.Powers.OfType<T>().FirstOrDefault()?.Amount ?? 0m;

            return state.MaterialTypeAmounts.GetValueOrDefault(materialType, 0m);
        }

        public static async Task LoseMaterial<T>(CardModel card, decimal amount, CardPlay? play = null)
            where T : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);
            if (amount <= 0m)
                return;

            if (!ShouldConsumeOnPlay(play))
                return;

            var owner = card.Owner?.Creature ??
                        throw new InvalidOperationException("Material source has no owner creature.");

            var consumeEvent = new MaterialConsumeEvent
            {
                Creature = owner,
                SourceCard = card,
                Deltas = [new(typeof(T), amount)],
                TotalAmount = amount,
            };
            await MaterialEventFlow.DispatchBeforeConsume(consumeEvent);
            await PowerCmd.Apply<T>(owner, -amount, owner, card);
            await MaterialEventFlow.DispatchAfterConsume(consumeEvent);
            await MaterialEventFlow.DispatchAfterResolved(new()
            {
                Creature = consumeEvent.Creature,
                SourceCard = consumeEvent.SourceCard,
                Deltas = consumeEvent.Deltas,
                TotalAmount = consumeEvent.TotalAmount,
                Kind = MaterialChangeKind.Consume,
                AppliedStressMultiplier = false,
            });
            CraftCmd.RecordMaterialConsume(owner);
        }

        public static async Task LoseMaterials<TFirst, TSecond>(CardModel card, decimal firstAmount,
            decimal secondAmount, CardPlay? play = null)
            where TFirst : MaterialPower
            where TSecond : MaterialPower
        {
            ArgumentNullException.ThrowIfNull(card);
            if (firstAmount <= 0m || secondAmount <= 0m)
                return;

            if (!ShouldConsumeOnPlay(play))
                return;

            var owner = card.Owner?.Creature ??
                        throw new InvalidOperationException("Material source has no owner creature.");

            var firstPower = owner.Powers.OfType<TFirst>()
                .FirstOrDefault(power => power.Amount >= firstAmount);
            var secondPower = owner.Powers.OfType<TSecond>()
                .FirstOrDefault(power => power.Amount >= secondAmount);

            if (firstPower == null || secondPower == null)
                return;

            var consumeEvent = new MaterialConsumeEvent
            {
                Creature = owner,
                SourceCard = card,
                Deltas = [new(typeof(TFirst), firstAmount), new(typeof(TSecond), secondAmount)],
                TotalAmount = firstAmount + secondAmount,
            };
            await MaterialEventFlow.DispatchBeforeConsume(consumeEvent);
            await PowerCmd.Apply<TFirst>(owner, -firstAmount, owner, card);
            await PowerCmd.Apply<TSecond>(owner, -secondAmount, owner, card);
            await MaterialEventFlow.DispatchAfterConsume(consumeEvent);
            await MaterialEventFlow.DispatchAfterResolved(new()
            {
                Creature = consumeEvent.Creature,
                SourceCard = consumeEvent.SourceCard,
                Deltas = consumeEvent.Deltas,
                TotalAmount = consumeEvent.TotalAmount,
                Kind = MaterialChangeKind.Consume,
                AppliedStressMultiplier = false,
            });
            CraftCmd.RecordMaterialConsume(owner);
        }

        private static Task CommitCardMaterialGains(CardModel card,
            IReadOnlyList<(Type Type, decimal Amount)> gains, bool applyStress = true)
        {
            return CommitCardMaterialGains(card.Owner.Creature, card, gains, applyStress);
        }

        private static async Task CommitCardMaterialGains(Creature creature, CardModel? card,
            IReadOnlyList<(Type Type, decimal Amount)> gains, bool applyStress = true)
        {
            ArgumentNullException.ThrowIfNull(creature);

            var filtered = gains.Where(g => g.Amount > 0m).ToList();
            if (filtered.Count == 0)
                return;

            var mult = applyStress && await TryTriggerStressPower(creature) ? 2m : 1m;

            var resolvedDeltas = filtered
                .Select(g => new MaterialDelta(g.Type, g.Amount * mult))
                .ToList();
            var totalGained = resolvedDeltas.Sum(d => d.Amount);
            var gainEvent = new MaterialGainEvent
            {
                Creature = creature,
                SourceCard = card,
                Deltas = resolvedDeltas,
                TotalAmount = totalGained,
                AppliedStressMultiplier = mult > 1m,
            };
            await MaterialEventFlow.DispatchBeforeGain(gainEvent);

            foreach (var delta in resolvedDeltas)
                await MaterialPowerRegistry.Apply(creature, delta.MaterialType, delta.Amount, card);

            await MaterialEventFlow.DispatchAfterGain(gainEvent);
            await MaterialEventFlow.DispatchAfterResolved(new()
            {
                Creature = gainEvent.Creature,
                SourceCard = gainEvent.SourceCard,
                Deltas = gainEvent.Deltas,
                TotalAmount = gainEvent.TotalAmount,
                Kind = MaterialChangeKind.Gain,
                AppliedStressMultiplier = gainEvent.AppliedStressMultiplier,
            });
        }

        private static async Task<bool> TryTriggerStressPower(Creature creature)
        {
            var stress = creature.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0);
            if (stress == null)
                return false;

            await PowerCmd.Apply<StressPower>(creature, -1m, creature, null);
            return true;
        }

        private static MaterialConsumeSeriesState EnsureSeriesState(CardModel card, CardPlay play)
        {
            var state = MaterialConsumeSeriesStates.GetOrCreate(card);
            if (play.IsFirstInSeries || !state.Initialized)
                state.Reset(IsFreePlay(play));

            return state;
        }

        private static bool ShouldConsumeOnPlay(CardPlay? play)
        {
            if (play == null)
                return true;

            return play.IsFirstInSeries && !IsFreePlay(play);
        }

        public static bool IsFreePlay(CardPlay play)
        {
            return play.IsAutoPlay || FreePlayBindingRegistry.IsFreeForPlay(play);
        }

        private static decimal GetTotalMaterials(Creature creature)
        {
            return creature.Powers
                .OfType<MaterialPower>()
                .Sum(p => p.Amount);
        }

        private sealed class MaterialConsumeSeriesState
        {
            public bool Initialized { get; private set; }

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public bool IsFreePlay { get; private set; }
            public bool HasAllMaterialsSnapshot { get; set; }
            public decimal AllMaterialsAmount { get; set; }
            public Dictionary<Type, decimal> MaterialTypeAmounts { get; } = new();

            public void Reset(bool isFreePlay)
            {
                Initialized = true;
                IsFreePlay = isFreePlay;
                HasAllMaterialsSnapshot = false;
                AllMaterialsAmount = 0m;
                MaterialTypeAmounts.Clear();
            }
        }
    }
}
