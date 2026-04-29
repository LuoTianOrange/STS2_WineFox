using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    /// <summary>
    ///     囤积习惯 - 1 cost Skill Common.
    ///     产出获得1木头1圆石。在下个回合开始时获得2木头2圆石。
    ///     升级：变为下回合开始时3木头3圆石。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class HoardingHabit() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            ModCardVars.Computed("Wood", 1m, _ => DynamicVars["Wood"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Wood")),
            ModCardVars.Computed("Stone", 1m, _ => DynamicVars["Stone"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Stone")),
            ModCardVars.Computed("NextTurnMaterial", 2m, _ => DynamicVars["NextTurnMaterial"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("NextTurnMaterial")),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardHoardingHabit);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var stressConsumed = await StressCmd.ConsumeOne(Owner.Creature, this);
            var mult = stressConsumed ? 2m : 1m;

            await MaterialCmd.GainMaterials<WoodPower, StonePower>(
                this,
                DynamicVars["Wood"].BaseValue * mult,
                DynamicVars["Stone"].BaseValue * mult,
                applyStress: false);

            var materialNextTurn = DynamicVars["NextTurnMaterial"].BaseValue * mult;
            await PowerCmd.Apply<HoardingHabitPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
                materialNextTurn,
                Owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["NextTurnMaterial"].UpgradeValueBy(1m);
        }
    }
}
