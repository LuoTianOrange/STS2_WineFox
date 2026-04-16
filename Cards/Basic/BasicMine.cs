using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Cards.Ancient;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Basic
{
    [RegisterCard(typeof(WineFoxCardPool))]
    [RegisterCharacterStarterCard(typeof(WineFox))]
    public class BasicMine() : WineFoxCard(1, CardType.Skill,
        CardRarity.Basic, TargetType.Self)
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

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBaseMine);

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
