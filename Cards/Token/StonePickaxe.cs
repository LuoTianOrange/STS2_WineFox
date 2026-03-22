using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token
{
    public class StonePickaxe() : WineFoxCard(0, CardType.Skill,
        CardRarity.Token, TargetType.Self, showInCardLibrary: false, autoAdd: false)
    {
        public override CardAssetProfile AssetProfile => new(
            Const.Paths.CardStonePickaxe,
            Const.Paths.CardStonePickaxe);
        
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<DiggingPower>("Digging", 1m)];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<DiggingPower>(Owner.Creature, DynamicVars["Digging"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Digging"].UpgradeValueBy(1m);
        }
    }
}
