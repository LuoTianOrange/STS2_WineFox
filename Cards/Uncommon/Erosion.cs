using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     溃蚀 - 1 cost Skill Uncommon.
    ///     给予 6 层瓦解（DisintegrationPower）。所有拥有瓦解的敌人失去与层数相同的生命。
    ///     升级：变为 10 层。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Erosion() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Stacks", 6m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<DemisePower>(),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardErosion);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var owner = Owner.Creature;
            if (owner.CombatState is not { } combatState) return;

            await PowerCmd.Apply<DemisePower>(new ThrowingPlayerChoiceContext(), play.Target, DynamicVars["Stacks"].BaseValue, owner, this);

            foreach (var enemy in combatState.HittableEnemies.ToList())
            {
                var disintegration = enemy.Powers.OfType<DemisePower>().FirstOrDefault();
                if (disintegration == null || disintegration.Amount <= 0m) continue;

                await CreatureCmd.Damage(
                    choiceContext,
                    enemy,
                    disintegration.Amount,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    owner,
                    this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Stacks"].UpgradeValueBy(4m);
        }
    }
}

