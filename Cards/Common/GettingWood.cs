using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    public class GettingWood() : WineFoxCard(
        1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            ModCardVars.Computed("Wood", 4m, _ => DynamicVars["Wood"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Wood")),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGettingWood);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var woodAmount = await MaterialCmd.ResolveCardMaterialAmount(
                Owner.Creature, this, DynamicVars["Wood"].BaseValue);

            await MaterialCmd.GainMaterial<WoodPower>(this, woodAmount, applyStress: false);

            await DamageCmd.Attack(woodAmount)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Wood"].UpgradeValueBy(2m);
        }
    }
}
