using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Token.HandCrank
{
    
    [RegisterCard(typeof(WineFoxTokenCardPool))]
    public class ObtainMaterials() : WineFoxCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IDirectApply
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone, WineFoxKeywords.Iron];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Wood", 1m),
            new("Stone", 3m),
            new("Iron", 0m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardObtainMaterials);
        
        public async Task Apply()
        {
            if (IsUpgraded)
            {
                await MaterialCmd.GainMaterials(Owner.Creature,
                [
                    (typeof(WoodPower), DynamicVars["Wood"].BaseValue),
                    (typeof(StonePower), DynamicVars["Stone"].BaseValue),
                    (typeof(IronPower), DynamicVars["Iron"].BaseValue)
                ], sourceCard: null, applyStress: true);
            }
            else
            {
                await MaterialCmd.GainMaterials<WoodPower, StonePower>(
                    Owner.Creature,
                    DynamicVars["Wood"].BaseValue,
                    DynamicVars["Stone"].BaseValue,
                    sourceCard: null,
                    applyStress: true);
            }
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            await Apply();
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Wood"].UpgradeValueBy(2m);
            DynamicVars["Iron"].UpgradeValueBy(3m);
        }
    }
}

