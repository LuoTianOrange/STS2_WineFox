using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class MultipleReinforcementPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.MultipleReinforcementPowerIcon);

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            if (Owner?.Player is not { } player)
                return;

            var bonus = Math.Max(1, (int)Amount);
            var slots = await MagicWineFoxSpellCmd.EnsurePower(player);
            slots?.ChangeBaseCapacity(bonus);
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext,
            Player player)
        {
            if (player.Creature != Owner)
                return;

            if (Owner.Player is not { } owner)
                return;

            var modifier = await SpellModifierFactory.GrantRandomToHand(choiceContext, owner);
            modifier?.SetToFreeThisTurn();
        }
    }
}
