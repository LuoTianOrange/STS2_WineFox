using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards
{
    public abstract class WineFoxCard(
        int baseCost,
        CardType type,
        CardRarity rarity,
        TargetType target,
        bool showInCardLibrary = true,
        bool autoAdd = true) : ModCardTemplate(baseCost, type, rarity, target, showInCardLibrary, autoAdd)
    {
        public override string? CustomEnergyIconPath => Const.Paths.EnergyIconCake;
    }
}
