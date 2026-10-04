using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    /// <summary>
    ///     全视之眼 —— 1 费技能法术（Common）。
    ///     <para>
    ///         装填后于回合结束释放：把一张**随机的法术修正**加入手牌。
    ///         本身带「消耗」关键字，因此装填打出后进入消耗堆，不再回到牌库循环。
    ///     </para>
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EyeOfProvidence() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        /// <summary>
        ///     「装填」+「释放」+「消耗」。
        ///     <para>
        ///         「消耗」由游戏根据关键字自动在卡面显示，本地化文案里不必再写一遍。
        ///     </para>
        /// </summary>
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            base.CanonicalKeywords.Append(CardKeyword.Exhaust);

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        public override string SpellIconPath => Const.Paths.SpellIconEyeOfProvidence;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        /// <summary>释放阶段：把一张随机的法术修正加入手牌。</summary>
        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner;
            if (owner?.Creature?.CombatState == null)
                return;

            await SpellModifierFactory.ChooseOneToHand(context.ChoiceContext, owner);
        }

        protected override void OnUpgrade()
        {
            // 无固定数值可升级。
        }
    }
}
