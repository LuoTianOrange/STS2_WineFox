using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    public class SophisticatedBackpack : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Uncommon;
        public override bool HasUponPickupEffect => true;

        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.SophisticatedBackpack);

        public override decimal ModifyHandDraw(Player player, decimal count)
        {
            if (player != Owner) return count;
            if (player.Creature.CombatState == null) return count;
            if (player.Creature.CombatState.RoundNumber > 1) return count;
            return count + 1;
        }
    }
}
