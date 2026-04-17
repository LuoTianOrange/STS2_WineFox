using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    /// <summary>
    ///     回忆 - 1 cost Power Uncommon.
    ///     在你的回合开始时，将你上一回合最后打出技能牌的一张消耗复制品加入你的手牌。
    ///     升级：复制品也会附加保留。
    /// </summary>
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Memory() : WineFoxCard(
        1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMemory);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            if (IsUpgraded)
            {
                var existing = Owner.Creature.Powers.OfType<MemoryPower>().FirstOrDefault();
                if (existing != null)
                    await PowerCmd.Remove(existing);
                await PowerCmd.Apply<MemoryPowerUpgraded>(Owner.Creature, 1m, Owner.Creature, this);
            }
            else
            {
                await PowerCmd.Apply<MemoryPower>(Owner.Creature, 1m, Owner.Creature, this);
            }
        }

        protected override void OnUpgrade() { }
    }
}
