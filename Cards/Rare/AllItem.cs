using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    public class AllItem() : WineFoxCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood,WineFoxKeywords.Stone,WineFoxKeywords.Iron,WineFoxKeywords.Diamond];
        
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new EnergyVar(1),new CardsVar(1),new("Wood", 1m),new DynamicVar("Stone",1m),new DynamicVar("Iron",1m),new DynamicVar("Diamond",1m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAllItem);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            
            await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
            
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
            
            await MaterialCmd.GainAllMaterials(Owner.Creature, 1m);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}