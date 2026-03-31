using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token
{
    public class DiamondArmor() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Armor", 7m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardDiamondArmor);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<PlatingPower>(
                Owner.Creature,
                DynamicVars["Armor"].BaseValue,
                Owner.Creature,
                this);

            await PowerCmd.Apply<DiamondArmorPower>(
                Owner.Creature,
                1m,
                Owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Armor"].UpgradeValueBy(2m);
        }
    }
}
