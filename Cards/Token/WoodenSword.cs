using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token
{
    public class WoodenSword() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.Self,
        showInCardLibrary: false, autoAdd: false)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Vigorous", 4m),new("Turns",2m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardWoodenSword,
            Const.Paths.CardWoodenSword);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<WoodenSwordPower>(
                Owner.Creature, 2m, Owner.Creature, this);
        }

        protected override void OnUpgrade() { }
    }
}