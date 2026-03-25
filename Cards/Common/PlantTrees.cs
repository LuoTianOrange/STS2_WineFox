using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    public class PlantTrees() : WineFoxCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        public override bool GainsBlock => true;

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new BlockVar(5, ValueProp.Move), new("Plant", 4m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPlantTrees);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            await PowerCmd.Apply<PlantPower>(
                Owner.Creature, DynamicVars["Plant"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Block"].UpgradeValueBy(2m); // 5 → 7
            DynamicVars["Plant"].UpgradeValueBy(2m);
        }
    }
}
