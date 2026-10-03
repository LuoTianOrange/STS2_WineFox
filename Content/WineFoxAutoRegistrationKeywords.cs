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

        [RegisterOwnedCardKeyword(WineFoxKeywords.PlatingKey,
            IconPath = "res://images/powers/plating_power.png")]
        private sealed class Plating;

        [RegisterOwnedCardKeyword(WineFoxKeywords.MaterialKey)]
        private sealed class Material;

        [RegisterOwnedCardKeyword(WineFoxKeywords.CraftKey)]
        private sealed class Craft;

        [RegisterOwnedCardKeyword(WineFoxKeywords.ExchangeKey)]
        private sealed class Exchange;

        [RegisterOwnedCardKeyword(WineFoxKeywords.MagicKey)]
        private sealed class Magic;

        /// <summary>
        ///     「装填」关键字：把牌放入法杖，回合结束时释放。
        ///     可装填的法术自动带此关键字，因此卡面文案不必重复解释装填的含义。
        /// </summary>
        [RegisterOwnedCardKeyword(WineFoxKeywords.LoadKey,
            IconPath = Const.Paths.EnergyIconCake)]
        private sealed class Load;

        /// <summary>
        ///     「释放」关键字：回合结束时，法杖中已装填的法术依次结算。
        ///     与「装填」成对出现，构成法杖体系的两半。
        /// </summary>
        [RegisterOwnedCardKeyword(WineFoxKeywords.ReleaseKey,
            IconPath = Const.Paths.EnergyIconCake)]
        private sealed class Release;

        [RegisterOwnedCardKeyword(WineFoxKeywords.SophisticatedBackpackKey,
            IconPath = Const.Paths.SophisticatedBackpack)]
        private sealed class SophisticatedBackpack;

        [RegisterOwnedCardKeyword(WineFoxKeywords.SwordKey)]
        private sealed class Sword;

        [RegisterOwnedCardKeyword(WineFoxKeywords.CookableFoodKey)]
        private sealed class CookableFood;

        [RegisterOwnedCardKeyword(WineFoxKeywords.CookedFoodKey)]
        private sealed class CookedFood;
    }
}
