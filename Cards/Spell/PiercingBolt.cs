using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    /// <summary>
    ///     穿刺魔弹 —— 1 费法术修正（Uncommon）。
    ///     <para>
    ///         装填后不施放，只修改「序列中紧随其后的那一张法术」：
    ///         把它的目标改为**对所有敌人**，但伤害**减少 20%**。
    ///     </para>
    ///     <para>
    ///         属于 <see cref="IMagicWineFoxSpellModifierCard" />，因此带「法术修正」关键字；
    ///         同时是可装填法术，带「装填」+「释放」。
    ///     </para>
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class PiercingBolt() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        /// <summary>伤害降低的百分比（做成 DynamicVar，卡面文案用 {DamageReduction:diff()} 引用）。</summary>
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("DamageReduction", 20m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        public override string SpellIconPath => Const.Paths.SpellIconPiercingBolt;

        /// <summary>法术修正符：下一个法术打全体，但伤害按 DamageReduction 百分比降低。</summary>
        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            var reduction = DynamicVars["DamageReduction"].BaseValue;
            modifiers.MarkTargetsAllEnemies();
            modifiers.MultiplyDamage(1m - reduction / 100m);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);
        }

        protected override void OnUpgrade()
        {
            // 修正符本身无固定数值可升——保持不可升级的效果说明。
        }
    }
}
