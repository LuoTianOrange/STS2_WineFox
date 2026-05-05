using STS2_WineFox.Cards;
using STS2RitsuLib.Interop.AutoRegistration;

namespace STS2_WineFox.Content
{
    internal static class WineFoxAutoRegistrationKeywords
    {
        [RegisterOwnedCardKeyword(WineFoxKeywords.DiggingKey,
            IconPath = Const.Paths.DiggingPowerIcon)]
        private sealed class Digging;

        [RegisterOwnedCardKeyword(WineFoxKeywords.WoodKey,
            IconPath = Const.Paths.WoodPowerIcon)]
        private sealed class Wood;

        [RegisterOwnedCardKeyword(WineFoxKeywords.StoneKey,
            IconPath = Const.Paths.StonePowerIcon)]
        private sealed class Stone;

        [RegisterOwnedCardKeyword(WineFoxKeywords.PlantKey,
            IconPath = Const.Paths.PlantPowerIcon)]
        private sealed class Plant;

        [RegisterOwnedCardKeyword(WineFoxKeywords.SteamKey,
            IconPath = Const.Paths.SteamPowerIcon)]
        private sealed class Steam;

        [RegisterOwnedCardKeyword(WineFoxKeywords.StressKey,
            IconPath = Const.Paths.StressPowerIcon)]
        private sealed class Stress;

        [RegisterOwnedCardKeyword(WineFoxKeywords.IronKey,
            IconPath = Const.Paths.IronPowerIcon)]
        private sealed class Iron;

        [RegisterOwnedCardKeyword(WineFoxKeywords.DiamondKey,
            IconPath = Const.Paths.DiamondPowerIcon)]
        private sealed class Diamond;

        [RegisterOwnedCardKeyword(WineFoxKeywords.StrengthKey,
            IconPath = "res://images/powers/strength_power.png")]
        private sealed class Strength;

        [RegisterOwnedCardKeyword(WineFoxKeywords.PlatingKey,
            IconPath = "res://images/powers/plating_power.png")]
        private sealed class Plating;

        [RegisterOwnedCardKeyword(WineFoxKeywords.MaterialKey)]
        private sealed class Material;

        [RegisterOwnedCardKeyword(WineFoxKeywords.CraftKey)]
        private sealed class Craft;

        [RegisterOwnedCardKeyword(WineFoxKeywords.ExchangeKey)]
        private sealed class Exchange;

        [RegisterOwnedCardKeyword(WineFoxKeywords.ChantKey,IconPath = Const.Paths.ChantPowerIcon)]
        private sealed class Chant;
    }
}
