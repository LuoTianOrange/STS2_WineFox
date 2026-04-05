using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token
{
    public class GoldenSword() : WineFoxCard(
        0, CardType.Power, CardRarity.Token, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new EnergyVar(1)];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGoldenSword);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<GoldenSwordPower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }
}
