using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.LessHoliday
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class WorkWork() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stress];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Stress", 1m), new CardsVar(0)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardWorkWork);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<StressPower>(Owner.Creature, DynamicVars["Stress"].BaseValue, Owner.Creature, this);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1m);
        }
    }
}
