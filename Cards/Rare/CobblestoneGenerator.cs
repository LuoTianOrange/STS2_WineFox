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
    [RegisterCard(typeof(WineFoxCardPool))]
    public class CobblestoneGenerator() : WineFoxCard(
        2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("BrushStoneFormPower", 2m)];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stone];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCobblestoneGenerator);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<BrushStoneFormPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["BrushStoneFormPower"].BaseValue,
                Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
