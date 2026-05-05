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
    ///     魔法飞弹 - 1 cost Skill Common.
    ///     敌人失去 5+ChantPower 点生命 2 次。获得 1 层 ChantPower。
    ///     升级：基础值 +3。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class MagicMissile() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            WineFoxCardVarFactory.ChantDamageVar(
                "Damage",
                5m),
            new PowerVar<ChantPower>(1m)
        ];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Chant];
        
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var damagePerHit = WineFoxCardVarFactory.ResolveChantDamageForPlay(this, "Damage", play.Target);
            for (var i = 0; i < 2; i++)
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    play.Target,
                    damagePerHit,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    Owner.Creature,
                    this);
            }

            await PowerCmd.Apply<ChantPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Damage"].UpgradeValueBy(3m);
        }
    }
}
