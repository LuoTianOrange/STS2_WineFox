/*
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class EssenceReconstruction() : WineFoxCard(
        1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEssenceReconstruction);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<ChantPower>()];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.MagicKeyword];
        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var creature = owner.Creature;

            var handCards = PileType.Hand.GetPile(owner).Cards.ToList();
            var exhaustedCount = handCards.Count;

            foreach (var card in handCards)
                await CardPileCmd.Add(card, PileType.Exhaust);

            var chantGain = exhaustedCount + (IsUpgraded ? 2m : 0m);
            if (chantGain > 0m)
                await PowerCmd.Apply<ChantPower>(choiceContext, creature, chantGain, creature, this);

            var drawAmount = creature.GetPowerAmount<ChantPower>();
            if (drawAmount > 0m)
                await CardPileCmd.Draw(choiceContext, drawAmount, owner);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
*/

namespace STS2_WineFox.Cards.Deleted.Magic
{
}
