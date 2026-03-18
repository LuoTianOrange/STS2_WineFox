using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Cards.DynamicVars;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Basic
{
    public class BasicMine() : WineFoxCard(1, CardType.Skill,
        CardRarity.Basic, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];
        
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new MineMaterialVar<WoodPower>("Wood", 2m),
            new MineMaterialVar<StonePower>("Stone", 2m),
        ];
        
        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardBaseMine,
            Const.Paths.CardBaseMine
        );
        
        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var diggingBonus = Owner.Creature.Powers.OfType<DiggingPower>().FirstOrDefault()?.Amount ?? 0m;

            await WineFoxActions.GainMaterials<WoodPower, StonePower>(
                this,
                DynamicVars["Wood"].BaseValue + diggingBonus,
                DynamicVars["Stone"].BaseValue + diggingBonus);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Wood"].UpgradeValueBy(1m);
            DynamicVars["Stone"].UpgradeValueBy(1m);
        }
    }
}
