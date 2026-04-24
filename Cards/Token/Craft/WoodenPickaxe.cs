using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.Craft
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class WoodenPickaxe() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardWoodenPickaxe);

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new EnergyVar(2), new CardsVar(1)];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var creature = Owner?.Creature;
            if (creature == null)
                return;

            var energyNextTurn = DynamicVars.Energy.BaseValue;
            var cardsNextTurn = DynamicVars.Cards.BaseValue;

            await PowerCmd.Apply<EnergyNextTurnPower>(new ThrowingPlayerChoiceContext(), creature, energyNextTurn, creature, this);
            await PowerCmd.Apply<DrawCardsNextTurnPower>(new ThrowingPlayerChoiceContext(), creature, cardsNextTurn, creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1m);
        }
    }
}
