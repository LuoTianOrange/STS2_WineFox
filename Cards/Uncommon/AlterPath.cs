using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class AlterPath() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.None)
    {
        public override bool GainsBlock => true;
        protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new BlockVar(5, ValueProp.Move), new CardsVar(2)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAlterPath);

        protected override bool IsPlayable =>
            Owner.Creature.Powers.OfType<WoodPower>().Any(p => (decimal)p.Amount >= 2);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var woodPower = Owner.Creature.Powers
                .OfType<WoodPower>()
                .FirstOrDefault(p => (decimal)p.Amount >= 2);
            if (woodPower == null) return;

            await PowerCmd.ModifyAmount(woodPower, -2m, null, this);
            CraftCmd.RecordMaterialConsume(Owner.Creature);
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, play);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Block"].UpgradeValueBy(3m);
        }
    }
}
