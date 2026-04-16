using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    public class PreProcessing() : WineFoxCard(
        0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            ModCardVars.Computed("Wood", 2m, _ => DynamicVars["Wood"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Wood")),
            ModCardVars.Computed("Stone", 2m, _ => DynamicVars["Stone"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Stone")),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Exhaust];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPreProcessing);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await MaterialCmd.GainMaterials<WoodPower, StonePower>(
                this,
                DynamicVars["Wood"].BaseValue,
                DynamicVars["Stone"].BaseValue);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Wood"].UpgradeValueBy(1m);
            DynamicVars["Stone"].UpgradeValueBy(1m);
        }
    }
}
