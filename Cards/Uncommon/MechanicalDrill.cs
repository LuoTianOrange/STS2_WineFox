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

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class MechanicalDrill() : WineFoxCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Iron, WineFoxKeywords.Stress];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(2m, ValueProp.Move),
            new IntVar("Hits", 3m),
            ModCardVars.Computed("Iron", 3m, _ => DynamicVars["Iron"].BaseValue,
                WineFoxCardVarFactory.StressDoubledDynamicVar("Iron")),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMechanicalDrill);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var stressPower = Owner.Creature.Powers
                .OfType<StressPower>()
                .FirstOrDefault(p => (decimal)p.Amount > 0);

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(DynamicVars["Hits"].IntValue)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            await MaterialCmd.GainMaterial<IronPower>(this, DynamicVars["Iron"].BaseValue);

            if (stressPower != null) Owner.PlayerCombatState?.Energy += 1;
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Iron"].UpgradeValueBy(1m); // 3 → 4
            DynamicVars.Damage.UpgradeValueBy(1m); // 2 → 3
        }
    }
}
