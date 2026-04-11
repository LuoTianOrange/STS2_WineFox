using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards;
using STS2_WineFox.Cards.Token.SophisticatedBackpack;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;

namespace STS2_WineFox.Relics.Backpack
{
    public sealed record SophisticatedBackpackUpgradeDefinition(
        Type ProductCardType,
        Func<CombatState, Player, CardModel> Factory,
        IReadOnlyList<CraftCost> Costs)
    {
        public CraftDeliveryMode DeliveryMode { get; init; } = CraftDeliveryMode.ImmediateEffect;
        public CardMultiplayerConstraint MultiplayerConstraint { get; init; } = CardMultiplayerConstraint.None;
    }

    public static class SophisticatedBackpackUpgradeCatalog
    {
        public static readonly IReadOnlyList<SophisticatedBackpackUpgradeDefinition> All =
        [
            new(
                typeof(RestockUpgrade),
                (state, owner) => state.CreateCard<RestockUpgrade>(owner),
                [
                    new(typeof(WoodPower), 8m),
                    new(typeof(IronPower), 6m),
                ]),
            new(
                typeof(FeedingUpgrade),
                (state, owner) => state.CreateCard<FeedingUpgrade>(owner),
                [
                    new(typeof(DiamondPower), 3m),
                    new(typeof(IronPower), 4m),
                ]),
            new(
                typeof(SmeltingUpgrade),
                (state, owner) => state.CreateCard<SmeltingUpgrade>(owner),
                [
                    new(typeof(StonePower), 8m),
                    new(typeof(IronPower), 5m),
                ]),
            new(
                typeof(CraftUpgrade),
                (state, owner) => state.CreateCard<CraftUpgrade>(owner),
                [
                    new(typeof(IronPower), 6m),
                    new(typeof(WoodPower), 4m),
                ]),
            new(
                typeof(StonecutterUpgrade),
                (state, owner) => state.CreateCard<StonecutterUpgrade>(owner),
                [
                    new(typeof(StonePower), 6m),
                    new(typeof(IronPower), 4m),
                    new(typeof(WoodPower), 4m),
                ]),
            new(
                typeof(SavingsUpgrade),
                (state, owner) => state.CreateCard<SavingsUpgrade>(owner),
                [
                    new(typeof(DiamondPower), 3m),
                    new(typeof(IronPower), 4m),
                ]),
        ];

        private static readonly IReadOnlyDictionary<Type, Type> EffectTypeByProductType = All
            .ToDictionary(upgrade => upgrade.ProductCardType, upgrade => ResolveEffectType(upgrade.ProductCardType));

        public static Type GetEffectType(Type productCardType)
        {
            return EffectTypeByProductType[productCardType];
        }

        private static Type ResolveEffectType(Type productCardType)
        {
            var current = productCardType;
            while (current != null)
            {
                if (current.IsGenericType &&
                    current.GetGenericTypeDefinition() == typeof(BackpackUpgradeCardBase<>))
                    return current.GetGenericArguments()[0];

                current = current.BaseType;
            }

            throw new InvalidOperationException($"升级卡 {productCardType.Name} 未继承 BackpackUpgradeCardBase<>。");
        }
    }
}
