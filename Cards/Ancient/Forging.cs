using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Ancient
{
    public class Forging() : WineFoxCard(
        1, CardType.Skill, CardRarity.Ancient, TargetType.Self)
    {
        public override bool GainsBlock => true;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(10m, ValueProp.Move),
            ModCardVars.Computed("Materials", 2m, _ => DynamicVars["Materials"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Materials")),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate, CardKeyword.Retain];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardForging);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            await CreatureCmd.GainBlock(owner.Creature, DynamicVars.Block, play);

            await MaterialCmd.GainAllMaterials(this, DynamicVars["Materials"].BaseValue);

            await CraftCmd.CraftIntoHand(choiceContext, this);
            await CraftCmd.CraftIntoHand(choiceContext, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(3m);
            EnergyCost.UpgradeBy(-1);
        }
    }
}
