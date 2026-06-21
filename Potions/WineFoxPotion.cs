using MegaCrit.Sts2.Core.Entities.Potions;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Potions
{
    public abstract class WineFoxPotion : ModPotionTemplate
    {
        public override PotionUsage Usage => PotionUsage.CombatOnly;

        protected static PotionAssetProfile Art(string imagePath, string? outlinePath = null)
        {
            return new(imagePath, outlinePath ?? imagePath);
        }
    }
}
