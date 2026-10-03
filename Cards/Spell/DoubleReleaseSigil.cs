using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    /// <summary>
    ///     双重释放符 —— 0 费修正符（衍生牌，由起始遗物【狐火杖】在战斗开始时白送 1 张）。
    ///     <para>
    ///         装填后**不施放**，只修改累积器：让其后的第一张法术额外释放 1 次，
    ///         且这次额外释放**不消耗**本轮释放的施法名额。
    ///     </para>
    ///     <para>
    ///         属于 <c>Token</c> 稀有度：不进牌池、不参与随机生成，只能由遗物产出。
    ///     </para>
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxTokenCardPool))]
    public class DoubleReleaseSigil() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardDoubleReleaseSigil);

        public override string SpellIconPath => Const.Paths.SpellIconDoubleReleaseSigil;

        /// <summary>修正符本身不需要施放，也不装填——它由遗物/效果直接放入槽位。</summary>
        public override bool IsLoadable => false;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.AddExtraCasts(1);
            modifiers.MarkExtraCastsBudgetFree();
        }

        /// <summary>作为手牌打出时：什么都没发生（设计上它只作为槽内修正符存在）。</summary>
        protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            return Task.CompletedTask;
        }

        protected override void OnUpgrade()
        {
            // 衍生牌不可升级。
        }
    }
}
