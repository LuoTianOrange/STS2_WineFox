using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.Craft
{
    [RegisterCard(typeof(WineFoxCraftingCardPool))]
    public class DiamondPickaxe() : WineFoxCard(
        0, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardDiamondPickaxe);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<DiamondPickaxePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, 1m,
                Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
