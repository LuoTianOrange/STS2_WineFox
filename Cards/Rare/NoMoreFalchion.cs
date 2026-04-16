using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Cards.Token.NoMoreFalchion;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class NoMoreFalchion() : WineFoxCard(
        2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<string> RegisteredKeywordIds =>
            [WineFoxKeywords.Iron];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromCard<SteelChamber>(IsUpgraded)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNoMoreFalchion);

        protected override bool IsPlayable =>
            Owner.Creature.Powers.OfType<IronPower>().Any(p => p.Amount > 0m);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var creature = owner.Creature;

            if (creature.CombatState is not { } combatState) return;

            var ironPower = creature.Powers.OfType<IronPower>().FirstOrDefault(p => p.Amount > 0m);
            if (ironPower == null) return;

            var ironAmount = ironPower.Amount;

            // 消耗所有铁
            await PowerCmd.ModifyAmount(ironPower, -ironAmount, null, this);

            // 生成 SteelChamber 并注入 hit 次数
            var steelChamber = combatState.CreateCard<SteelChamber>(owner);
            steelChamber.DynamicVars["Hits"].BaseValue = ironAmount;

            if (IsUpgraded)
                CardCmd.Upgrade(steelChamber);

            await CardPileCmd.AddGeneratedCardToCombat(steelChamber, PileType.Hand, true);
        }

        protected override void OnUpgrade()
        {
            // 升级后生成已升级的 SteelChamber（OnPlay 中处理）
        }
    }
}
