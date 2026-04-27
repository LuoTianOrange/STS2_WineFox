using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.Craft
{
    [RegisterCard(typeof(WineFoxCraftingCardPool))]
    public class StonePickaxe() : WineFoxCard(0, CardType.Power,
        CardRarity.Token, TargetType.Self, false)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardStonePickaxe);

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new PowerVar<DiggingPower>("Digging", 2m), new PowerVar<WoodPower>(2m), new PowerVar<StonePower>(2m)];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<DiggingPower>(new ThrowingPlayerChoiceContext(), Owner.Creature,
                DynamicVars["Digging"].BaseValue, Owner.Creature, this);

            if (IsUpgraded)
                await MaterialCmd.GainMaterials<WoodPower, StonePower>(this, DynamicVars["WoodPower"].BaseValue,
                    DynamicVars["StonePower"].BaseValue);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
