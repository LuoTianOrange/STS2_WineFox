using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.MultiMachinery
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class FineProcessing() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IMultiMachineryChoice
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Count", 2m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardFineProcessing);

        public async Task Apply(PlayerChoiceContext choiceContext, CardModel sourceCard)
        {
            var owner = sourceCard.Owner.Creature;
            await PowerCmd.Apply<FineProcessingPower>(owner, DynamicVars["Count"].BaseValue, owner, sourceCard);
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await Apply(choiceContext, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Count"].UpgradeValueBy(1m);
        }
    }
}
