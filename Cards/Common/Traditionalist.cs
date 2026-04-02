using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    public class Traditionalist() : WineFoxCard(
        0, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Craft];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new CardsVar(1)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardTraditionalist);

        protected override bool IsPlayable
        {
            get
            {
                var wood = Owner.Creature.Powers.OfType<WoodPower>().FirstOrDefault()?.Amount ?? 0;
                return wood >= 2m;
            }
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var ownerCreature = Owner.Creature;
            if (ownerCreature.CombatState is null) return;

            var woodPower = ownerCreature.Powers.OfType<WoodPower>().FirstOrDefault();
            if (woodPower == null || woodPower.Amount < 2m) return;

            await PowerCmd.ModifyAmount(woodPower, -2m, null, this);
            await CraftCmd.CraftIntoHand(choiceContext, this);
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Cards.UpgradeValueBy(1);
        }
    }
}
