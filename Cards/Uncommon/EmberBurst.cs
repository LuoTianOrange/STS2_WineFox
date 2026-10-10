using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EmberBurst() : WineFoxCard(
        2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(10m, ValueProp.Move),
            new("Burn", 4m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEmberBurst);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<BurningPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner?.Creature?.CombatState is not { } combatState)
                return;

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, play)
                .TargetingAllOpponents(combatState)
                .Execute(choiceContext);

            var burn = DynamicVars["Burn"].BaseValue;
            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                if (!enemy.IsAlive)
                    continue;

                await PowerCmd.Apply<BurningPower>(
                    choiceContext,
                    enemy,
                    burn,
                    Owner.Creature,
                    this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3m);
            DynamicVars["Burn"].UpgradeValueBy(2m);
        }
    }
}
