using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class InequivalentExchange() : WineFoxCard(
        2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardInequivalentExchange);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var combatState = owner.Creature.CombatState;
            if (combatState == null) return;

            var handCards = PileType.Hand.GetPile(owner).Cards
                .Where(c => !ReferenceEquals(c, this))
                .ToList();

            if (handCards.Count == 0) return;

            foreach (var card in handCards)
                await CardPileCmd.Add(card, PileType.Exhaust);

            var rng = owner.RunState.Rng.CombatCardGeneration;
            for (var i = 0; i < handCards.Count; i++)
            {
                var creator = rng.NextItem(RareCardCreators);
                if (creator == null) continue;

                var card = creator(combatState, owner);
                if (IsUpgraded && !card.IsUpgraded)
                    CardCmd.Upgrade(card);

                var instance = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, owner);
                CardCmd.PreviewCardPileAdd(instance);
            }
        }

        protected override void OnUpgrade()
        {
        }

        private static readonly Func<ICombatState, Player, CardModel>[] RareCardCreators =
        [
            (state, owner) => state.CreateCard<AccumulatingGrudges>(owner),
            (state, owner) => state.CreateCard<AllItem>(owner),
            (state, owner) => state.CreateCard<ArmToTeeth>(owner),
            (state, owner) => state.CreateCard<AutoCrafter>(owner),
            (state, owner) => state.CreateCard<BatchCraft>(owner),
            (state, owner) => state.CreateCard<BladeMaster>(owner),
            (state, owner) => state.CreateCard<CobblestoneGenerator>(owner),
            (state, owner) => state.CreateCard<EasyPeasy>(owner),
            (state, owner) => state.CreateCard<HellGift>(owner),
            (state, owner) => state.CreateCard<LessHoliday>(owner),
            (state, owner) => state.CreateCard<Liberation>(owner),
            (state, owner) => state.CreateCard<MaidSupport>(owner),
            (state, owner) => state.CreateCard<Milk>(owner),
            (state, owner) => state.CreateCard<MiningGems>(owner),
            (state, owner) => state.CreateCard<NetheriteChestPlate>(owner),
            (state, owner) => state.CreateCard<NetheriteSword>(owner),
            (state, owner) => state.CreateCard<NoMoreFalchion>(owner),
            (state, owner) => state.CreateCard<PaybackTime>(owner),
            (state, owner) => state.CreateCard<PlanningExpert>(owner),
            (state, owner) => state.CreateCard<ShieldAttack>(owner),
            (state, owner) => state.CreateCard<SpinningHand>(owner),
            (state, owner) => state.CreateCard<SpiritFoxForm>(owner),
            (state, owner) => state.CreateCard<SteamEngine>(owner),
            (state, owner) => state.CreateCard<WirelessTerminal>(owner),
        ];
    }
}
