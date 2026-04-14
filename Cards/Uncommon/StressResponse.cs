using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class StressResponse() : WineFoxCard(
        0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new IntVar("HpLoss", 6m),
            new MaxHpVar(1m),
            new EnergyVar(2),
            new CardsVar(2),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardStressResponse);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var creature = owner.Creature;

            var hpLoss = DynamicVars["HpLoss"].BaseValue;
            var maxHpGain = DynamicVars.MaxHp.BaseValue;
            var energyGain = DynamicVars.Energy.IntValue;
            var cardsToDraw = DynamicVars.Cards.BaseValue;

            await CreatureCmd.GainMaxHp(creature, maxHpGain);
            
            await CreatureCmd.Damage(
                choiceContext ?? new ThrowingPlayerChoiceContext(),
                creature,
                hpLoss,
                ValueProp.Unblockable | ValueProp.Unpowered,
                null,
                this);

            await PlayerCmd.GainEnergy(energyGain, owner);

            if (choiceContext == null) return;
            await CardPileCmd.Draw(choiceContext, cardsToDraw, owner);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Energy.UpgradeValueBy(1m);
        }
    }
}
