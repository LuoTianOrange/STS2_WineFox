using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class MechanicalSaw() : WineFoxCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        private static readonly AttachedState<CardModel, StressConsumeSeriesState> StressConsumeSeriesStates =
            new(() => new());

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stress];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(8m, ValueProp.Move), new IntVar("BonusDamage", 7m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMechanicalSaw);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var damage = DynamicVars.Damage.BaseValue;
            var state = StressConsumeSeriesStates.GetOrCreate(this);
            var consumedStressThisSeries = state.ConsumedStress;

            if (play.IsFirstInSeries)
            {
                consumedStressThisSeries = false;
                if (!MaterialCmd.IsFreePlay(play))
                {
                    var stressPower = owner.Powers.OfType<StressPower>().FirstOrDefault(p => p.Amount > 0m);
                    if (stressPower != null)
                    {
                        await PowerCmd.ModifyAmount(stressPower, -1m, null, this);
                        consumedStressThisSeries = true;
                    }
                }

                state.ConsumedStress = consumedStressThisSeries;
            }

            if (consumedStressThisSeries)
                damage += DynamicVars["BonusDamage"].BaseValue;

            await DamageCmd.Attack(damage)
                .FromCard(this)
                .TargetingAllOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3m);
            DynamicVars["BonusDamage"].UpgradeValueBy(2m);
        }

        private sealed class StressConsumeSeriesState
        {
            public bool ConsumedStress { get; set; }
        }
    }
}
