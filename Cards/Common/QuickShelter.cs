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
    public class QuickShelter() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(5m, ValueProp.Move),
            new CardsVar(1),
            ModCardVars.Computed("Wood", 1m, _ => DynamicVars["Wood"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Wood")),
            ModCardVars.Computed("Stone", 1m, _ => DynamicVars["Stone"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Stone")),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardQuickShelter);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var creature = owner.Creature;

            await MaterialCmd.GainMaterials<WoodPower, StonePower>(
                this,
                DynamicVars["Wood"].BaseValue,
                DynamicVars["Stone"].BaseValue);

            await CreatureCmd.GainBlock(creature, DynamicVars.Block, play);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Wood"].UpgradeValueBy(1m); // 1 → 2
            DynamicVars["Stone"].UpgradeValueBy(1m); // 1 → 2
        }
    }
}
