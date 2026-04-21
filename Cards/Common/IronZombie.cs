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
    public class IronZombie() : WineFoxCard(
        1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(6m, ValueProp.Move),
            ModCardVars.Computed("Iron", 2m, _ => DynamicVars["Iron"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Iron")),
        ];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Iron];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardIronZombie);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            await MaterialCmd.GainMaterial<IronPower>(this, DynamicVars["Iron"].BaseValue);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(4m);
        }
    }
}
