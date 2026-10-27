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
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class PreservedSalvation() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Turns", 1m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardPreservedSalvation);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<PreservedEnergyPower>(),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner?.Creature is not { } creature)
                return;

            await PowerCmd.Apply<PreservedEnergyPower>(
                choiceContext,
                creature,
                DynamicVars["Turns"].BaseValue,
                creature,
                this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Turns"].UpgradeValueBy(1m);
        }
    }
}
