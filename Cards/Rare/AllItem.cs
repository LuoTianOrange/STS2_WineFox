using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class AllItem() : WineFoxCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone, WineFoxKeywords.Iron, WineFoxKeywords.Diamond];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new EnergyVar(1),
            new CardsVar(1),
            ModCardVars.Computed("Wood", 1m, _ => DynamicVars["Wood"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Wood")),
            ModCardVars.Computed("Stone", 1m, _ => DynamicVars["Stone"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Stone")),
            ModCardVars.Computed("Iron", 1m, _ => DynamicVars["Iron"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Iron")),
            ModCardVars.Computed("Diamond", 1m, _ => DynamicVars["Diamond"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Diamond")),
        ];

        public override CardAssetProfile AssetProfile => IsUpgradable ? Art(Const.Paths.CardAllItemUpgraded) : Art(Const.Paths.CardAllItem);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);

            await MaterialCmd.GainAllMaterials(this, 1m);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}
