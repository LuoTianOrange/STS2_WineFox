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
    public class Loan() : WineFoxCard(
        0, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CardsVar(3),
            new("DrawLess", 2m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLoan);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var owner = Owner;
            if (owner?.Creature == null)
                return;

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);

            await PowerCmd.Apply<LoanPower>(
                choiceContext,
                owner.Creature,
                DynamicVars["DrawLess"].BaseValue,
                owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["DrawLess"].UpgradeValueBy(-1m);
        }
    }
}
