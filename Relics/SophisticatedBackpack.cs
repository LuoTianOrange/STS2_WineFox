using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    public class SophisticatedBackpack : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Uncommon;

        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.SophisticatedBackpack);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("DrawBonus", 1m), new("RestockApplied", 0m)];

        public bool IsRestockUpgradeApplied => DynamicVars["RestockApplied"].BaseValue > 0m;

        public void ApplyRestockUpgrade()
        {
            DynamicVars["DrawBonus"].BaseValue += 2m;
            DynamicVars["RestockApplied"].BaseValue = 1m;
            Flash();
        }

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (player != Owner) return count;
            if(player.Creature.CombatState == null) return count;
            if (player.Creature.CombatState.RoundNumber > 1) return count;
            return count + DynamicVars["DrawBonus"].BaseValue;
        }
    }
}
