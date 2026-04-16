using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class MiningGems() : WineFoxCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            ModCardVars.Computed("Diamond", 2m, _ => DynamicVars["Diamond"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Diamond")),
        ];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Diamond];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMiningGems);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await MaterialCmd.GainMaterial<DiamondPower>(this, DynamicVars["Diamond"].BaseValue);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Diamond"].UpgradeValueBy(1m); // 2 → 3
        }
    }
}
