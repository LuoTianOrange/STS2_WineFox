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
    public class SteamEngine() : WineFoxCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stress];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Steam", 1m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSteamEngine);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<SteamPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars["Steam"].BaseValue, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1); // 2 → 1
        }
    }
}
