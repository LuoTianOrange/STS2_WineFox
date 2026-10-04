using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    /// <summary>
    ///     序列回响 —— 2 费能力牌（Rare）。
    ///     <para>
    ///         你的法术槽 +1；消耗你的下一张法术卡，每当你释放法杖时打出该法术
    ///         （参考 Noita 的「始终释放」）。升级后额外抽 1 张法术牌。
    ///     </para>
    /// </summary>
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EchoesSequence() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        /// <summary>法术槽增加量。经 Power 的 <c>Amount</c> 传给它，保证代码与文案同一来源。</summary>
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("SlotBonus", 1m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await PowerCmd.Apply<EchoesSequencePower>(
                choiceContext,
                Owner.Creature,
                DynamicVars["SlotBonus"].BaseValue,
                Owner.Creature,
                this);

            if (IsUpgraded)
                await DrawOneSpell(choiceContext);
        }

        /// <summary>升级效果：从抽牌堆里抽 1 张可装填的法术牌。</summary>
        private async Task DrawOneSpell(PlayerChoiceContext choiceContext)
        {
            var drawPile = PileType.Draw.GetPile(Owner);
            var spell = drawPile.Cards.FirstOrDefault(card =>
                card is Cards.Spell.MagicWineFoxSpellCard { IsLoadable: true });

            if (spell == null)
                return;

            await CardPileCmd.Add(spell, PileType.Hand);
        }

        protected override void OnUpgrade()
        {
            // 升级效果写在 OnPlay 里（额外抽 1 张法术牌），无数值可加。
        }
    }
}
