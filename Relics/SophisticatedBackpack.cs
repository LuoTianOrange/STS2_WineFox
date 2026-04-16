using System.Globalization;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Relics.Backpack;
using STS2_WineFox.Relics.Backpack.Effects;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public class SophisticatedBackpack : WineFoxRelic, ICraftListener
    {
        private readonly Dictionary<string, int> _effectStateInts = new();
        private bool _pendingEffectFlash;

        public override RelicRarity Rarity => RelicRarity.Rare;

        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.SophisticatedBackpack);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            BuildCanonicalVars();

        [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
        public string? DynamicVarSaveData
        {
            get => BuildDynamicVarSaveData();
            set => ApplyDynamicVarSaveData(value);
        }

        [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
        public string? EffectStateSaveData
        {
            get => BuildEffectStateSaveData();
            set => ApplyEffectStateSaveData(value);
        }

        public Task BeforeCraft(CraftExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public Task BeforeCraftProductDelivered(CraftExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public async Task AfterCraftProductDelivered(CraftExecutionContext context)
        {
            _pendingEffectFlash = false;
            foreach (var effect in SophisticatedBackpackEffects.All)
            {
                if (!HasEffect(effect))
                    continue;

                await effect.AfterCraftProductDelivered(this, context);
            }

            FlushPendingEffectFlash();
        }

        public void NotifyBackpackEffectTriggered()
        {
            _pendingEffectFlash = true;
        }

        private void FlushPendingEffectFlash()
        {
            if (!_pendingEffectFlash)
                return;

            _pendingEffectFlash = false;
            Flash();
        }

        private IEnumerable<DynamicVar> BuildCanonicalVars()
        {
            var vars = new List<DynamicVar>
            {
                new SophisticatedBackpackDescriptionVar(() => SophisticatedBackpackEffects.BuildDescription(this)),
            };

            foreach (var effect in SophisticatedBackpackEffects.All)
            {
                foreach (var var in effect.CreateCanonicalVars())
                    vars.Add(var.Clone());

                vars.Add(new(
                    SophisticatedBackpackEffects.EnabledVar(effect.GetType()),
                    effect.EnabledByDefault ? 1m : 0m));
            }

            return vars;
        }

        public bool HasEffect(ISophisticatedBackpackEffect effect)
        {
            ArgumentNullException.ThrowIfNull(effect);
            return HasEffect(effect.GetType());
        }

        public bool HasEffect(Type effectType)
        {
            var enabledVar = SophisticatedBackpackEffects.EnabledVar(effectType);
            if (!DynamicVars.TryGetValue(enabledVar, out var state))
                return false;

            return state.BaseValue > 0m;
        }

        public bool HasEffect<TEffect>() where TEffect : ISophisticatedBackpackEffect
        {
            return HasEffect(typeof(TEffect));
        }

        public bool TryApplyUpgrade(Type effectType, Action<SophisticatedBackpack>? onApplied = null)
        {
            if (!SophisticatedBackpackEffects.ByType.ContainsKey(effectType))
                return false;

            var enabledVar = SophisticatedBackpackEffects.EnabledVar(effectType);
            if (DynamicVars[enabledVar].BaseValue > 0m)
                return false;

            _pendingEffectFlash = false;
            DynamicVars[enabledVar].BaseValue = 1m;
            onApplied?.Invoke(this);
            RefreshDescriptionText();
            InvokeDisplayAmountChanged();
            NotifyBackpackEffectTriggered();
            FlushPendingEffectFlash();
            return true;
        }

        public override Task AfterObtained()
        {
            RefreshDescriptionText();
            return Task.CompletedTask;
        }

        public bool TryApplyUpgrade<TEffect>(Action<SophisticatedBackpack>? onApplied = null)
            where TEffect : ISophisticatedBackpackEffect
        {
            return TryApplyUpgrade(typeof(TEffect), onApplied);
        }

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (player != Owner)
                return count;

            _pendingEffectFlash = false;
            var result = count;
            foreach (var effect in SophisticatedBackpackEffects.All)
            {
                if (!HasEffect(effect))
                    continue;

                result = effect.ModifyHandDraw(this, player, result);
            }

            FlushPendingEffectFlash();
            return result;
        }

        public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
            CombatState combatState)
        {
            _pendingEffectFlash = false;
            foreach (var effect in SophisticatedBackpackEffects.All)
            {
                if (!HasEffect(effect))
                    continue;

                await effect.BeforeSideTurnStart(this, choiceContext, side, combatState);
            }

            FlushPendingEffectFlash();
        }

        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            _pendingEffectFlash = false;
            foreach (var effect in SophisticatedBackpackEffects.All)
            {
                if (!HasEffect(effect))
                    continue;

                await effect.AfterTurnEnd(this, choiceContext, side);
            }

            FlushPendingEffectFlash();
        }

        public override async Task AfterCombatEnd(CombatRoom room)
        {
            _effectStateInts.Clear();

            _pendingEffectFlash = false;
            foreach (var effect in SophisticatedBackpackEffects.All)
            {
                if (!HasEffect(effect))
                    continue;

                await effect.AfterCombatEnd(this, room);
            }

            FlushPendingEffectFlash();
        }

        public int GetEffectStateInt(Type effectType, string key)
        {
            return _effectStateInts.GetValueOrDefault(StateKey(effectType, key), 0);
        }

        public int GetEffectStateInt<TEffect>(string key) where TEffect : ISophisticatedBackpackEffect
        {
            return GetEffectStateInt(typeof(TEffect), key);
        }

        public void SetEffectStateInt(Type effectType, string key, int value)
        {
            _effectStateInts[StateKey(effectType, key)] = value;
        }

        public void SetEffectStateInt<TEffect>(string key, int value) where TEffect : ISophisticatedBackpackEffect
        {
            SetEffectStateInt(typeof(TEffect), key, value);
        }

        public int IncrementEffectStateInt(Type effectType, string key, int delta = 1)
        {
            var stateKey = StateKey(effectType, key);
            var value = _effectStateInts.GetValueOrDefault(stateKey, 0) + delta;
            _effectStateInts[stateKey] = value;
            return value;
        }

        public int IncrementEffectStateInt<TEffect>(string key, int delta = 1)
            where TEffect : ISophisticatedBackpackEffect
        {
            return IncrementEffectStateInt(typeof(TEffect), key, delta);
        }

        private static string StateKey(Type effectType, string key)
        {
            return $"{effectType.FullName}:{key}";
        }

        public void RefreshDescriptionText()
        {
            if (DynamicVars[SophisticatedBackpackEffects.DescriptionVar] is SophisticatedBackpackDescriptionVar
                descriptionVar)
            {
                descriptionVar.Refresh();
                return;
            }

            ((StringVar)DynamicVars[SophisticatedBackpackEffects.DescriptionVar]).StringValue =
                SophisticatedBackpackEffects.BuildDescription(this);
        }

        private string? BuildDynamicVarSaveData()
        {
            var entries = new List<string>();
            foreach (var effect in SophisticatedBackpackEffects.All)
            {
                var enabledVar = SophisticatedBackpackEffects.EnabledVar(effect.GetType());
                if (!DynamicVars.TryGetValue(enabledVar, out var enabledState))
                    continue;

                var expectedEnabled = effect.EnabledByDefault ? 1m : 0m;
                if (enabledState.BaseValue != expectedEnabled)
                    entries.Add($"{enabledVar}={enabledState.BaseValue.ToString(CultureInfo.InvariantCulture)}");

                foreach (var templateVar in effect.CreateCanonicalVars())
                {
                    if (!DynamicVars.TryGetValue(templateVar.Name, out var currentState))
                        continue;

                    if (currentState.BaseValue != templateVar.BaseValue)
                        entries.Add(
                            $"{templateVar.Name}={currentState.BaseValue.ToString(CultureInfo.InvariantCulture)}");
                }
            }

            return entries.Count > 0 ? string.Join(";", entries) : null;
        }

        private void ApplyDynamicVarSaveData(string? saveData)
        {
            if (string.IsNullOrWhiteSpace(saveData))
                return;

            foreach (var pair in saveData.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var separator = pair.IndexOf('=');
                if (separator <= 0 || separator >= pair.Length - 1)
                    continue;

                var varName = pair[..separator];
                var rawValue = pair[(separator + 1)..];
                if (!decimal.TryParse(rawValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                    continue;

                if (DynamicVars.TryGetValue(varName, out var dynamicVar))
                    dynamicVar.BaseValue = value;
            }

            RefreshDescriptionText();
            InvokeDisplayAmountChanged();
        }

        private string? BuildEffectStateSaveData()
        {
            var entries = _effectStateInts
                .Where(entry => entry.Value != 0)
                .Select(entry => $"{entry.Key}={entry.Value.ToString(CultureInfo.InvariantCulture)}")
                .ToList();
            return entries.Count > 0 ? string.Join(";", entries) : null;
        }

        private void ApplyEffectStateSaveData(string? saveData)
        {
            _effectStateInts.Clear();
            if (string.IsNullOrWhiteSpace(saveData))
                return;

            foreach (var pair in saveData.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var separator = pair.LastIndexOf('=');
                if (separator <= 0 || separator >= pair.Length - 1)
                    continue;

                var key = pair[..separator];
                var rawValue = pair[(separator + 1)..];
                if (!int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                    continue;

                if (value != 0)
                    _effectStateInts[key] = value;
            }

            RefreshDescriptionText();
        }
    }
}
