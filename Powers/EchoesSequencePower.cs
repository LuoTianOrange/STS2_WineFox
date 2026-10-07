using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    [RegisterPower]
    public class EchoesSequencePower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.EchoesSequencePowerIcon);

        private CardModel? _echoSpell;
        private bool _awaitingSpell;

        private CardModel? _boundCardInstance;

        public bool IsAwaitingSpell => _awaitingSpell;

        public CardModel? EchoSpell => _echoSpell;

        internal void AddBoundSpellArg(LocString description)
        {
            if (_echoSpell == null)
            {
                description.Add("BoundSpellClause", string.Empty);
                return;
            }

            var clause = new LocString("powers", $"{Id.Entry}.boundClause");
            clause.Add("SpellName", new LocString("cards", $"{_echoSpell.Id.Entry}.title").GetFormattedText());

            description.Add("BoundSpellClause", clause.GetFormattedText());
        }

        public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            _awaitingSpell = true;

            if (Owner?.Player is not { } player)
                return;

            var bonus = Math.Max(1, (int)Amount);
            var slots = await MagicWineFoxSpellCmd.EnsurePower(player);
            slots?.EnsureCapacity(slots.SlotCapacity + bonus);
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (_boundCardInstance == null || !ReferenceEquals(cardPlay.Card, _boundCardInstance))
                return;

            _boundCardInstance = null;
            await CardPileCmd.Add(cardPlay.Card, PileType.Exhaust);
        }

        public bool TryBind(CardModel card)
        {
            if (!_awaitingSpell || !IsBindable(card))
                return false;

            _echoSpell = card.CreateClone();
            _boundCardInstance = card;
            _awaitingSpell = false;
            return true;
        }

        private static bool IsBindable(CardModel? card)
        {
            return card is MagicWineFoxSpellCard { IsLoadable: true };
        }
    }
}
