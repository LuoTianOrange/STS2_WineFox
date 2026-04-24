using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.HellGift
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class GoldenPickaxe() : WineFoxCard(
        0, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGoldenPickaxe);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var multiplier = IsUpgraded ? 3m : 2m;
            await PowerCmd.Apply<GoldenPickaxePower>(new ThrowingPlayerChoiceContext(), Owner.Creature, multiplier, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
