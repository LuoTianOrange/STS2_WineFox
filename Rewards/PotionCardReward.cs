using System.Text.Json;
using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Combat.Rewards;

namespace STS2_WineFox.Rewards
{
    internal sealed class PotionCardReward : CardReward, IModSerializableReward
    {
        private const string RewardId = "POTION_CARD";
        private const int DefaultOptionCount = 3;

        private static RewardType _registeredRewardType = RewardType.None;

        private readonly int _optionCount;
        private readonly CardRarity _rarity;

        private PotionCardReward(CardRarity rarity, int optionCount, Player player)
            : base(CreateOptions(player, rarity), optionCount, player)
        {
            _rarity = rarity;
            _optionCount = optionCount;
        }

        protected override RewardType RewardType => ModRewardType;

        public RewardType ModRewardType
        {
            get
            {
                if (_registeredRewardType == RewardType.None)
                    throw new InvalidOperationException("Potion card reward was used before registration.");

                return _registeredRewardType;
            }
        }

        internal static void InitializeRegistration()
        {
            if (_registeredRewardType != RewardType.None)
                return;

            var definition = ModRewardRegistry.For(Const.ModId).RegisterOwned<PotionCardRewardSaveData>(
                RewardId,
                PotionCardRewardJsonContext.Default.PotionCardRewardSaveData,
                (_, player, payload) => FromSaveData(player, payload));
            _registeredRewardType = definition.RewardType;
        }

        internal static PotionCardReward Create(CardRarity rarity, Player player, int optionCount = DefaultOptionCount)
        {
            return new(ValidateRarity(rarity), NormalizeOptionCount(optionCount), player);
        }

        public string? ToModRewardJson()
        {
            var payload = new PotionCardRewardSaveData
            {
                Rarity = _rarity,
                OptionCount = _optionCount,
            };
            return JsonSerializer.Serialize(payload, PotionCardRewardJsonContext.Default.PotionCardRewardSaveData);
        }

        public override SerializableReward ToSerializable()
        {
            return ModRewardSerialization.CreateSerializable(this);
        }

        private static PotionCardReward FromSaveData(Player player, PotionCardRewardSaveData? payload)
        {
            if (payload == null)
            {
                Main.Logger.Warn("[PotionCardReward] Missing save payload; restoring the default uncommon reward.");
                return Create(CardRarity.Uncommon, player);
            }

            try
            {
                return Create(payload.Rarity, player, payload.OptionCount);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Main.Logger.Warn(
                    $"[PotionCardReward] Invalid save payload ({ex.Message}); restoring the default uncommon reward.");
                return Create(CardRarity.Uncommon, player);
            }
        }

        private static CardCreationOptions CreateOptions(Player player, CardRarity rarity)
        {
            return CardCreationOptions
                .ForNonCombatWithUniformOdds(
                    [player.Character.CardPool],
                    card => card.Rarity == rarity)
                .WithFlags(CardCreationFlags.NoRarityModification | CardCreationFlags.NoCardPoolModifications);
        }

        private static CardRarity ValidateRarity(CardRarity rarity)
        {
            return rarity switch
            {
                CardRarity.Uncommon or CardRarity.Rare => rarity,
                _ => throw new ArgumentOutOfRangeException(nameof(rarity), rarity,
                    "Potion card rewards only support uncommon and rare cards."),
            };
        }

        private static int NormalizeOptionCount(int optionCount)
        {
            return optionCount > 0 ? optionCount : DefaultOptionCount;
        }
    }

    internal sealed class PotionCardRewardSaveData
    {
        public CardRarity Rarity { get; set; }
        public int OptionCount { get; set; }
    }

    [JsonSerializable(typeof(PotionCardRewardSaveData))]
    internal sealed partial class PotionCardRewardJsonContext : JsonSerializerContext;
}
