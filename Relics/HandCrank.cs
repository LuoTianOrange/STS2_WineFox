using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    public class HandCrank : WineFoxRelic
    {
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.HandCrankRelicIcon);
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Stress", 1m),];
        public override RelicRarity Rarity => RelicRarity.Starter;

        public override async Task BeforeCombatStart()
        {
            Flash();
            await PowerCmd.Apply<StressPower>(Owner.Creature, DynamicVars["Stress"].BaseValue, Owner.Creature, null);
        }
    }
}
