using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    /// <summary>
    ///     屏障波 - 1 cost Skill Common.
    ///     使所有敌人失去 5 点生命。获得 5 点格挡。获得 1 点[gold]吟唱[/gold]。
    ///     升级：敌人失去 9 点生命，获得 7 点格挡。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class BarrierWave() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            WineFoxCardVarFactory.ChantDamageVar(
                "Damage",
                5m),
            WineFoxCardVarFactory.PowerAmountVar<ChantPower>(1m),
            WineFoxCardVarFactory.BlockAmountVar(5m),
        ];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Chant];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBarrierWave);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var damage = WineFoxCardVarFactory.ResolveChantDamageForPlay(this, "Damage", null);
            foreach (var enemy in combatState.HittableEnemies.ToList())
                await CreatureCmd.Damage(
                    choiceContext,
                    enemy,
                    damage,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    owner,
                    this);

            var chantToGain = WineFoxCardVarFactory.ChantScaledPowerAmount<ChantPower>(this);
            var blockToGain = WineFoxCardVarFactory.ChantScaledAmount(this, "Block");

            await PowerCmd.Apply<ChantPower>(choiceContext, owner, chantToGain, owner, this);
            await CreatureCmd.GainBlock(owner, blockToGain, ValueProp.Move, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Damage"].UpgradeValueBy(4m);
            DynamicVars.Block.UpgradeValueBy(2m);
        }
    }
}
