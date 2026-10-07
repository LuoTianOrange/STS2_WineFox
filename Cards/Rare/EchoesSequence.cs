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
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EchoesSequence() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("SlotBonus", 1m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEchoesSequence);

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
