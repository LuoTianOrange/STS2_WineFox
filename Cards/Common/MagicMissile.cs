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
using MegaCrit.Sts2.Core.Models;

namespace STS2_WineFox.Cards.Common
{
    /// <summary>
    ///     魔法飞弹 - 1 cost Skill Common.
    ///     使敌人失去 3 点生命 3 次。
    ///     升级：基础值 +2（变为 5）。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class MagicMissile() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            WineFoxCardVarFactory.ChantDamageVar(
                "Damage",
                3m),
            new PowerVar<ChantPower>(1m)
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<ChantPower>()];
        
        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.MagicKeyword];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var damagePerHit = WineFoxCardVarFactory.ResolveChantDamageForPlay(this, "Damage", play.Target);
            for (var i = 0; i < 3; i++)
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    play.Target,
                    damagePerHit,
                    ValueProp.Unblockable | ValueProp.Unpowered,
                    Owner.Creature,
                    this);
            }

            await PowerCmd.Apply<ChantPower>(Owner.Creature, 1m, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Damage"].UpgradeValueBy(2m);
        }
    }
}
