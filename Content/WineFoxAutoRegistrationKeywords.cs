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

        /// <summary>
        ///     装填关键字：把牌放入法杖，回合结束时释放。
        /// </summary>
        [RegisterOwnedCardKeyword(WineFoxKeywords.LoadKey)]
        private sealed class Load;

        /// <summary>
        ///     释放关键字：回合结束时，法杖中已装填的法术依次结算。
        /// </summary>
        [RegisterOwnedCardKeyword(WineFoxKeywords.ReleaseKey)]
        private sealed class Release;

        /// <summary>
        ///     法术修正关键字：装入法杖后不施放，只修改法术的释放方式。
        /// </summary>
        [RegisterOwnedCardKeyword(WineFoxKeywords.SpellModifierKey)]
        private sealed class SpellModifier;

        [RegisterOwnedCardKeyword(WineFoxKeywords.SophisticatedBackpackKey,
            IconPath = Const.Paths.SophisticatedBackpack)]
        private sealed class SophisticatedBackpack;

        [RegisterOwnedCardKeyword(WineFoxKeywords.SwordKey)]
        private sealed class Sword;

        [RegisterOwnedCardKeyword(WineFoxKeywords.CookableFoodKey)]
        private sealed class CookableFood;

        [RegisterOwnedCardKeyword(WineFoxKeywords.CookedFoodKey)]
        private sealed class CookedFood;
        
        [RegisterOwnedCardKeyword(WineFoxKeywords.ChargeKey)]
        private sealed class Charge;
    }
}
