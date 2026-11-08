using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    /// <summary>
    ///     下回合开始时把登记的牌以费用减 1 的形式放回手牌。
    /// </summary>
    [RegisterPower]
    public class SpellReturnPower : WineFoxPower
    {
        private readonly List<CardModel> _pending = [];

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.SpellReturnPowerIcon);

        public bool Bind(CardModel card)
        {
            if (_pending.Any(entry => ReferenceEquals(entry, card)))
                return false;

            _pending.Add(card);
            return true;
        }

        public static async Task RegisterAsync(
            PlayerChoiceContext choiceContext,
            Creature creature,
            CardModel card,
            CardModel? cardSource)
        {
            var existing = creature.Powers.OfType<SpellReturnPower>().FirstOrDefault();

            if (existing == null)
            {
                var created = await PowerCmd.Apply<SpellReturnPower>(
                    choiceContext, creature, 1m, creature, cardSource);
                created?.Bind(card);
                return;
            }

            existing.Bind(card);
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext,
            Player player)
        {
            if (player.Creature != Owner)
                return;

            var pending = _pending.ToList();
            _pending.Clear();

            foreach (var card in pending)
            {
                var copy = card.CreateClone();
                copy.EnergyCost.AddThisCombat(-1);

                CardCmd.PreviewCardPileAdd(
                    await CardPileCmd.AddGeneratedCardToCombat(copy, PileType.Hand, player));
            }

            if (pending.Count > 0)
                Flash();

            await PowerCmd.Remove(this);
        }
    }
}
