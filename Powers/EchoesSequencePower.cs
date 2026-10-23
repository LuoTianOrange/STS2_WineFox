using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class EchoesSequencePower : WineFoxPower
    {
        private CardModel? _boundCardInstance;

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.EchoesSequencePowerIcon);

        public bool IsAwaitingSpell { get; private set; }

        public CardModel? EchoSpell { get; private set; }

        internal void AddBoundSpellArg(LocString description)
        {
            if (EchoSpell == null)
            {
                description.Add("BoundSpellClause", string.Empty);
                return;
            }

            var clause = new LocString("powers", $"{Id.Entry}.boundClause");
            clause.Add("SpellName", new LocString("cards", $"{EchoSpell.Id.Entry}.title").GetFormattedText());

            description.Add("BoundSpellClause", clause.GetFormattedText());
        }

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            IsAwaitingSpell = true;

            if (Owner?.Player is not { } player)
                return;

            var bonus = Math.Max(1, Amount);
            var slots = await MagicWineFoxSpellCmd.EnsurePower(player);
            slots?.ChangeBaseCapacity(bonus);
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (_boundCardInstance == null || !ReferenceEquals(cardPlay.Card, _boundCardInstance))
                return;

            _boundCardInstance = null;
            await CardCmd.Exhaust(context, cardPlay.Card);
        }

        public bool TryBind(CardModel card)
        {
            if (!IsAwaitingSpell || !IsBindable(card))
                return false;

            EchoSpell = card.CreateClone();
            _boundCardInstance = card;
            IsAwaitingSpell = false;
            return true;
        }

        private static bool IsBindable(CardModel? card)
        {
            return card is MagicWineFoxSpellCard { IsLoadable: true }
                and not IMagicWineFoxSpellModifierCard;
        }
    }
}
