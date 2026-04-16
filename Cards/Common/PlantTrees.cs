using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class PlantTrees() : WineFoxCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        public override bool GainsBlock => true;

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(5, ValueProp.Move),
            ModCardVars.Computed("Plant", 4m, _ => DynamicVars["Plant"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Plant")),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPlantTrees);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            var woodNextTurn = await MaterialCmd.ResolveCardMaterialAmount(
                Owner.Creature,
                this,
                DynamicVars["Plant"].BaseValue);
            await PowerCmd.Apply<PlantPower>(Owner.Creature, woodNextTurn, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Block"].UpgradeValueBy(2m); // 5 → 7
            DynamicVars["Plant"].UpgradeValueBy(2m);
        }
    }
}
