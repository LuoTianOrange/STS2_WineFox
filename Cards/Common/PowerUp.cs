using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class PowerUp() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(1), new("Stress", 1m)];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stress];
        
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPowerUp);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<StressPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["Stress"].BaseValue, Owner.Creature, this);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1m);
        }
    }
}
