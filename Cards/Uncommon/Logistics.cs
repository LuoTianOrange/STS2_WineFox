using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Logistics() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardLogistics);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var ownerCreature = owner.Creature;

            var target = play.Target;
            if (target == null) return;
            if (!target.IsPlayer || !target.IsAlive) return;
            if (ReferenceEquals(target, ownerCreature)) return;
            if (target.Player == null) return;

            var handCards = PileType.Hand.GetPile(owner).Cards;
            if (handCards.Count == 0) return;

            var options = IsUpgraded ? handCards : handCards.Where(c => c != this).ToList();
            if (options.Count == 0) return;

            var prompt = new LocString("cards", "STS2_WINE_FOX_CARD_LOGISTICS_CHOOSE");
            var prefs = new CardSelectorPrefs(prompt, 1, 1);

            var selectedList = await CardSelectCmd.FromHand(choiceContext, owner, prefs, null, this);
            var chosen = selectedList.FirstOrDefault();
            if (chosen == null) return;

            if (!IsUpgraded && ReferenceEquals(chosen, this))
                return;

            var clone = chosen.CreateClone();

            var combatState = ownerCreature.CombatState;
            if (combatState != null)
            {
                combatState.RemoveCard(clone);
                combatState.AddCard(clone, target.Player);
            }

            var instance = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, target.Player);
            if (LocalContext.IsMe(target))
                CardCmd.PreviewCardPileAdd(instance);


            await CardPileCmd.Add(chosen, PileType.Exhaust);
        }

        protected override void OnUpgrade()
        {
        }
    }
}


