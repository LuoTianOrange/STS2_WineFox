using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Cards.Token.Craft;
using STS2_WineFox.Cards.Token.HellGift;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    /// <summary>
    ///     下界合金剑 - 2 cost Attack Rare.
    ///     将你消耗牌堆中的所有剑（木剑、石剑、铁剑、钻石剑）打出。
    ///     升级效果：将消耗牌堆中的所有剑升级后打出。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class NetheriteSword() : WineFoxCard(
        2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNetheriteSword);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;
            var exhaustCards = PileType.Exhaust.GetPile(owner).Cards.ToList();

            var swords = exhaustCards
                .Where(c => c is WoodenSword or StoneSword or IronSword or DiamondSword or GoldenSword)
                .ToList();

            if (swords.Count == 0) return;

            var target = play.Target ?? owner.Creature.CombatState?.Enemies.FirstOrDefault(e => e.IsAlive);

            foreach (var sword in swords)
            {
                var copy = sword.CreateDupe();
                if (IsUpgraded && !copy.IsUpgraded)
                    CardCmd.Upgrade(copy);

                await CardCmd.AutoPlay(choiceContext, copy, target);
            }
        }

        protected override void OnUpgrade()
        {
            // Upgrade effect is handled in OnPlay (upgrading the swords before playing).
        }
    }
}

