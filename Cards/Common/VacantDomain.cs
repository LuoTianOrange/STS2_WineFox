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
    public class VacantDomain() : WineFoxCard(
        3, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stone];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(14m, ValueProp.Move),
            ModCardVars.Computed("StonePower", 8m, _ => DynamicVars["StonePower"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("StonePower")),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardVacantDomain);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            await MaterialCmd.GainMaterial<StonePower>(this, DynamicVars["StonePower"].BaseValue);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Block"].UpgradeValueBy(4m);
            DynamicVars["StonePower"].UpgradeValueBy(4m);
        }
    }
}
