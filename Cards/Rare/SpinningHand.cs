using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class SpinningHand() : WineFoxCard(
        4, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        private int _appliedCraftsThisCombat;

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Craft];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new DamageVar(20m, ValueProp.Move)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSpinningHand);


        public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.Card.Owner != Owner)
                return Task.CompletedTask;

            var ownerCreature = Owner?.Creature;
            if (ownerCreature == null)
                return Task.CompletedTask;

            var totalCrafts = CraftCmd.GetCraftCountThisCombat(ownerCreature);

            var delta = totalCrafts - _appliedCraftsThisCombat;
            if (delta <= 0)
                return Task.CompletedTask;

            EnergyCost.AddThisCombat(-delta);

            _appliedCraftsThisCombat = totalCrafts;
            return Task.CompletedTask;
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            if (Owner.Creature.CombatState is not { } combatState) return;

            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .TargetingAllOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        public override Task AfterCardEnteredCombat(CardModel card)
        {
            if (card != this || IsClone)
                return Task.CompletedTask;

            var ownerCreature = Owner?.Creature;
            if (ownerCreature == null)
                return Task.CompletedTask;

            var totalCrafts = CraftCmd.GetCraftCountThisCombat(ownerCreature);
            if (totalCrafts <= 0)
                return Task.CompletedTask;

            var delta = totalCrafts - _appliedCraftsThisCombat;
            if (delta <= 0)
                return Task.CompletedTask;

            EnergyCost.AddThisCombat(-delta);
            _appliedCraftsThisCombat = totalCrafts;
            return Task.CompletedTask;
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(6m);
        }
    }
}
