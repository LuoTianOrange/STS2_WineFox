using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     充能类法术的共用登记与重装逻辑：登记快照，下回合开始时装回法术槽，
    ///     槽位不足则塞进抽牌堆。子类只负责自己的图标与本地化。
    /// </summary>
    public abstract class RechargeSpellPowerBase : WineFoxPower
    {
        private readonly List<MagicWineFoxSpellSlotSnapshot> _pending = [];

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        /// <summary>这张法术是否已经登记过。身份用源卡判定，克隆件每次释放都是新对象。</summary>
        public bool IsRegistered(MagicWineFoxSpellSlotSnapshot snapshot)
        {
            var identity = snapshot.SourceCard ?? snapshot.Card;

            return _pending.Any(entry => ReferenceEquals(entry.SourceCard ?? entry.Card, identity));
        }

        /// <summary>登记一张待重装的法术，已经登记过的返回 false。</summary>
        public bool Bind(MagicWineFoxSpellSlotSnapshot snapshot)
        {
            if (IsRegistered(snapshot))
                return false;

            _pending.Add(snapshot);
            return true;
        }

        /// <summary>把一张法术登记到指定充能能力上，同一张牌被额外释放时不会重复登记。</summary>
        public static async Task RegisterAsync<TPower>(
            PlayerChoiceContext choiceContext,
            Creature creature,
            MagicWineFoxSpellSlotSnapshot snapshot,
            CardModel? cardSource) where TPower : RechargeSpellPowerBase
        {
            var existing = creature.Powers.OfType<TPower>().FirstOrDefault();

            if (existing == null)
            {
                var created = await PowerCmd.Apply<TPower>(
                    choiceContext, creature, 1m, creature, cardSource);
                created?.Bind(snapshot);
                return;
            }

            existing.Bind(snapshot);
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext,
            Player player)
        {
            if (player.Creature != Owner)
                return;

            var pending = _pending.ToList();
            _pending.Clear();

            var slots = await MagicWineFoxSpellCmd.EnsurePower(player, choiceContext);

            foreach (var snapshot in pending)
            {
                if (slots != null && slots.TryLoad(snapshot))
                    continue;

                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(
                        snapshot.Card,
                        PileType.Draw,
                        player,
                        CardPilePosition.Random));
            }

            if (pending.Count > 0)
                Flash();

            await PowerCmd.Remove(this);
        }
    }
}
