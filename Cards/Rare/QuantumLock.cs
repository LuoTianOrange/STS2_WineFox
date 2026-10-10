using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    /// <summary>
    ///     量子锁定：造成伤害，并使本回合你的法术无法被格挡。
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class QuantumLock() : WineFoxCard(
        1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(12m, ValueProp.Move)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardQuantumLock);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (play.Target is not { } target || Owner?.Creature is not { } caster)
                return;
            
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, play)
                .Targeting(target)
                .Execute(choiceContext);

            await PowerCmd.Apply<QuantumLockPower>(
                choiceContext,
                caster,
                1m,
                caster,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(4m);
        }
    }
}
