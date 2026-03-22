using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token
{
    public class IronArmor() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.Self,
        showInCardLibrary: false, autoAdd: false)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Armor", 5m)];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardIronArmor,
            Const.Paths.CardIronArmor);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<PlatingPower>(
                Owner.Creature, DynamicVars["Armor"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade() { }
    }
}