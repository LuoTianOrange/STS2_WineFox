using STS2_WineFox.Cards;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Content
{
    internal static class WineFoxAutoRegistrationKeywords
    {
        [RegisterOwnedCardKeyword(WineFoxKeywords.DiggingKey, LocKeyPrefix = "STS2_WINEFOX-DIGGING",
            IconPath = Const.Paths.DiggingPowerIcon)]
        private sealed class Digging;

        [RegisterOwnedCardKeyword(WineFoxKeywords.WoodKey, LocKeyPrefix = "STS2_WINEFOX-WOOD",
            IconPath = Const.Paths.WoodPowerIcon)]
        private sealed class Wood;

        [RegisterOwnedCardKeyword(WineFoxKeywords.StoneKey, LocKeyPrefix = "STS2_WINEFOX-STONE",
            IconPath = Const.Paths.StonePowerIcon)]
        private sealed class Stone;

        [RegisterOwnedCardKeyword(WineFoxKeywords.PlantKey, LocKeyPrefix = "STS2_WINEFOX-PLANT",
            IconPath = Const.Paths.PlantPowerIcon)]
        private sealed class Plant;

        [RegisterOwnedCardKeyword(WineFoxKeywords.SteamKey, LocKeyPrefix = "STS2_WINEFOX-STEAM",
            IconPath = Const.Paths.SteamPowerIcon)]
        private sealed class Steam;

        [RegisterOwnedCardKeyword(WineFoxKeywords.StressKey, LocKeyPrefix = "STS2_WINEFOX-STRESS",
            IconPath = Const.Paths.StressPowerIcon)]
        private sealed class Stress;

        [RegisterOwnedCardKeyword(WineFoxKeywords.IronKey, LocKeyPrefix = "STS2_WINEFOX-IRON",
            IconPath = Const.Paths.IronPowerIcon)]
        private sealed class Iron;

        [RegisterOwnedCardKeyword(WineFoxKeywords.DiamondKey, LocKeyPrefix = "STS2_WINEFOX-DIAMOND",
            IconPath = Const.Paths.DiamondPowerIcon)]
        private sealed class Diamond;

        [RegisterOwnedCardKeyword(WineFoxKeywords.StrengthKey, LocKeyPrefix = "STS2_WINEFOX-STRENGTH",
            IconPath = "res://images/powers/strength_power.png")]
        private sealed class Strength;

        [RegisterOwnedCardKeyword(WineFoxKeywords.PlatingKey, LocKeyPrefix = "STS2_WINEFOX-PLATING",
            IconPath = "res://images/powers/plating_power.png")]
        private sealed class Plating;

        [RegisterOwnedCardKeyword(WineFoxKeywords.MaterialKey, LocKeyPrefix = "STS2_WINEFOX-MATERIAL")]
        private sealed class Material;

        [RegisterOwnedCardKeyword(WineFoxKeywords.CraftKey, LocKeyPrefix = "STS2_WINEFOX-CRAFT")]
        private sealed class Craft;

        [RegisterOwnedCardKeyword(WineFoxKeywords.ExchangeKey, LocKeyPrefix = "STS2_WINEFOX-EXCHANGE")]
        private sealed class Exchange;
    }
}
