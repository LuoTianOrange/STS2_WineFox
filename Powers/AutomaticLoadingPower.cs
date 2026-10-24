using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class AutomaticLoadingPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.AutomaticLoadingPowerIcon);

        public override async Task AfterSideTurnEnd(
            PlayerChoiceContext choiceContext,
            CombatSide side,
            IEnumerable<Creature> participants)
        {
            if (side != Owner.Side)
                return;

            if (Owner.Player is not { } player)
                return;

            var spell = PileType.Hand.GetPile(player).Cards.LastOrDefault(card =>
                card is MagicWineFoxSpellCard { IsLoadable: true } and not IMagicWineFoxSpellModifierCard);

            if (spell == null)
                return;

            var slots = await MagicWineFoxSpellCmd.EnsurePower(player, choiceContext);
            if (slots == null)
                return;

            var snapshot = new MagicWineFoxSpellSlotSnapshot(
                spell.CreateClone(),
                null,
                false,
                0,
                spell);

            if (!slots.TryLoad(snapshot))
                return;

            Flash();

            await CardPileCmd.Add(spell, PileType.Discard);
        }
    }
}
