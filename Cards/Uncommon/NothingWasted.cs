using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class NothingWasted() : WineFoxCard(
        1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Block", 1m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNothingWasted);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await PowerCmd.Apply<NothingWastedPower>(
                Owner.Creature,
                DynamicVars["Block"].BaseValue,
                Owner.Creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Block"].UpgradeValueBy(1m); // 1 -> 2
        }
    }
}
