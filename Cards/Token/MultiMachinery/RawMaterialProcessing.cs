using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.MultiMachinery
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class RawMaterialProcessing() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IMultiMachineryChoice
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Amount", 1m)];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.MaterialKeyword];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRawMaterialProcessing);

        public async Task Apply(PlayerChoiceContext choiceContext, CardModel sourceCard)
        {
            var owner = sourceCard.Owner.Creature;
            var amount = DynamicVars["Amount"].BaseValue;
            var gains = MaterialPowerRegistry.RegisteredMaterialTypes
                .Where(type => owner.Powers.Any(power => power.GetType() == type && power.Amount > 0m))
                .Select(type => (type, amount));

            await MaterialCmd.GainMaterials(owner, gains, sourceCard, applyStress: false);
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await Apply(choiceContext, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Amount"].UpgradeValueBy(1m);
        }
    }
}
