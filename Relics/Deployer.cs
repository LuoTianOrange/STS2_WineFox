using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Token.HandCrank;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public class Deployer : HandCrank
    {
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.DeployerRelicIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Stress", 3m),
            new("Wood", 3m),
            new("Stone", 3m),
            new("Iron", 3m)
        ];

        protected override List<CardModel> CreateOptions(ICombatState combatState)
        {
            var stress = combatState.CreateCard(ModelDb.Card<ObtainStress>(), Owner);
            var materials = combatState.CreateCard(ModelDb.Card<ObtainMaterials>(), Owner);
            CardCmd.Upgrade(stress);
            CardCmd.Upgrade(materials);
            return [stress, materials];
        }
    }
}
