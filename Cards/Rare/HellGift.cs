using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Token.HellGift;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    public class HellGift() : WineFoxCard(
        2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardHellGift);

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<GoldenSword>(IsUpgraded),
            HoverTipFactory.FromCard<GoldenPickaxe>(IsUpgraded),
        ];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            if (owner.Creature.CombatState is not { } combatState) return;
            if (CombatState == null) return;

            Func<CardModel>[] goldenCardCreators =
            [
                () => CombatState.CreateCard<GoldenSword>(owner),
                () => CombatState.CreateCard<GoldenPickaxe>(owner),
                // () => CombatState.CreateCard<GoldenAxe>(owner),
                () => CombatState.CreateCard<GoldenArmor>(owner),
            ];

            // 随机选一张
            var creator = combatState.RunState.Rng.CombatTargets.NextItem(goldenCardCreators);
            if (creator == null) return;
            var card = creator();

            // 升级后：将选中的金质牌升级
            if (IsUpgraded)
                CardCmd.Upgrade(card);

            var instance = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
            CardCmd.PreviewCardPileAdd(instance);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
