using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    public class QuickShelter() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Wood, WineFoxKeywords.Stone];
        
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];
        
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(10m, ValueProp.Move),
            new CardsVar(1)
        ];

        protected override bool IsPlayable =>
            Owner.Creature.Powers.OfType<WoodPower>().Any(p => (decimal)p.Amount >= 1m) &&
            Owner.Creature.Powers.OfType<StonePower>().Any(p => (decimal)p.Amount >= 1m);
        
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardQuickShelter);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var creature = owner.Creature;
            var hasWood = creature.Powers.OfType<WoodPower>().Any(p => p.Amount >= 1m);
            var hasStone = creature.Powers.OfType<StonePower>().Any(p => p.Amount >= 1m);

            if (!hasWood || !hasStone)
            {
                return;
            }

            await MaterialCmd.LoseMaterials<WoodPower, StonePower>(this, 1m, 1m);

            await CreatureCmd.GainBlock(creature, DynamicVars.Block, play);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Block"].UpgradeValueBy(3m);
        }
    }
}