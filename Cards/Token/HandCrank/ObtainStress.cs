using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.HandCrank
{
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class ObtainStress() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IDirectApply
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Stress];
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new("Stress", 1m)];
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardObtainStress);
        
        public async Task Apply()
        {
            await PowerCmd.Apply<StressPower>(Owner.Creature, DynamicVars["Stress"].BaseValue, Owner.Creature, this);
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await Apply();
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Stress"].UpgradeValueBy(2m);
        }
    }
}
