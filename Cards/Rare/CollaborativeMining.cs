using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class CollaborativeMining() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            WineFoxKeywords.WoodKeyword,
            WineFoxKeywords.StoneKeyword,
            WineFoxKeywords.IronKeyword,
            WineFoxKeywords.DiamondKeyword,
        ];

        public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCollaborativeMining);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var creature = Owner.Creature;
            await PowerCmd.Apply<CollaborativeMiningPower>(new ThrowingPlayerChoiceContext(), creature, 1m, creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}
