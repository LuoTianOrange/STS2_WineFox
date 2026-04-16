using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    /// <summary>
    ///     魔法飞弹 - 1 cost Skill Common.
    ///     敌人失去 6 点生命 2 次。升级：变为 8 点。
    /// </summary>
    public class MagicMissile() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Damage", 6m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var damage = DynamicVars["Damage"].BaseValue;
            for (var i = 0; i < 2; i++)
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    play.Target,
                    damage,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    Owner.Creature,
                    this);
            }
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Damage"].UpgradeValueBy(2m);
        }
    }
}

