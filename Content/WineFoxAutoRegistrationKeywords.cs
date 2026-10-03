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
        ///     法杖/法术体系的关键字（v0.2.6 法杖构筑版）。
        ///     与既有 <c>magic</c>（魔法/咏唱管线）刻意分开：法术走自己的装填-铸法链，
        ///     不参与 <c>MagicDamage</c>/<c>ChantPower</c> 那条咏唱管线。
        ///     图标暂用奥术能量色占位，等美术资源到位再替换。
        /// </summary>
        [RegisterOwnedCardKeyword(WineFoxKeywords.SpellKey,
            IconPath = Const.Paths.EnergyIconCake)]
        private sealed class Spell;
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
