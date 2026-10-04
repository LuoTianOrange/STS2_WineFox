using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    /// <summary>
    ///     定向爆破 —— 1 费攻击牌（Common）。
    ///     <para>
    ///         不装填：打出时**立即释放**法杖中已装填的法术，并以选定敌人作为释放目标，
    ///         随后抽 1 张牌。升级后改为抽 2 张。
    ///     </para>
    ///     <para>
    ///         实现要点：法术本身是 <c>TargetType.None</c>，装填快照里不记录目标，
    ///         因此把选定的敌人作为 <c>fallbackTarget</c> 传给
    ///         <see cref="MagicWineFoxSpellCmd.CastAll" />，由 <c>ResolveTarget</c> 的回落逻辑接管；
    ///         若某张法术装填时自带目标，则仍优先用自己记录的目标。
    ///     </para>
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class DirectionalBlasting() : WineFoxCard(
        1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CardsVar(1)
        ];

        /// <summary>带「释放」关键字——这张牌主动触发法杖释放。</summary>
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            WineFoxKeywords.ReleaseKeyword
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            // 立即释放：把法杖里已装填的法术全部导向选定的敌人。
            await MagicWineFoxSpellCmd.CastAll(choiceContext, Owner, play.Target, this);

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}
