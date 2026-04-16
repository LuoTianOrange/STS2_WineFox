using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    /// <summary>
    ///     屏障波 - 1 cost Skill Common.
    ///     所有敌人失去 6 点生命，获得 5 点格挡。
    ///     升级：敌人失去 9 点生命，获得 7 点格挡。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class BarrierWave() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new IntVar("Damage", 6m),
            new BlockVar(5m, ValueProp.Move),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBarrierWave);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            var damage = DynamicVars["Damage"].BaseValue;
            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    enemy,
                    damage,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    owner,
                    this);
            }

            await CreatureCmd.GainBlock(owner, DynamicVars.Block, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Damage"].UpgradeValueBy(3m);
            DynamicVars.Block.UpgradeValueBy(2m);
        }
    }
}

