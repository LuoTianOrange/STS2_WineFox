using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class RechargeSpellPower : WineFoxPower
    {
        private readonly List<MagicWineFoxSpellSlotSnapshot> _pending = [];

        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.RechargeSpellPowerIcon);

        public void Bind(MagicWineFoxSpellSlotSnapshot snapshot)
        {
            _pending.Add(snapshot);
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
