using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
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
    public class ReproductionPower : WineFoxPower
    {
        private bool _awaitingSpell;

        private MagicWineFoxSpellSlotSnapshot? _boundSpell;
        public override PowerType Type => PowerType.Buff;

        public override PowerStackType StackType => PowerStackType.Single;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.ReproductionPowerIcon);

        /// <summary>当前绑定的法术</summary>
        public CardModel? BoundSpell => _boundSpell?.Card;

        public void Bind(MagicWineFoxSpellSlotSnapshot? snapshot)
        {
            _boundSpell = snapshot;
            _awaitingSpell = snapshot == null;
        }

        public bool TryBind(MagicWineFoxSpellSlotSnapshot snapshot)
        {
            if (!_awaitingSpell || !IsBindable(snapshot.Card))
                return false;

            _boundSpell = snapshot;
            _awaitingSpell = false;
            return true;
        }

        public static bool IsBindable(CardModel? card)
        {
            return card is MagicWineFoxSpellCard { IsLoadable: true }
                and not IMagicWineFoxSpellModifierCard;
        }

        internal void AddBoundSpellArg(LocString description)
        {
            if (_boundSpell?.Card is not { } spell)
            {
                description.Add("BoundSpellClause", string.Empty);
                return;
            }

            var clause = new LocString("powers", $"{Id.Entry}.boundClause");
            clause.Add("SpellName", new LocString("cards", $"{spell.Id.Entry}.title").GetFormattedText());

            description.Add("BoundSpellClause", clause.GetFormattedText());
        }

        protected override async Task OnAfterPlayerTurnStart(
            PlayerChoiceContext choiceContext,
            Player player)
        {
            if (player.Creature != Owner)
                return;

            var snapshot = _boundSpell;
            _boundSpell = null;
            _awaitingSpell = false;

            if (snapshot != null)
            {
                var slots = await MagicWineFoxSpellCmd.EnsurePower(player, choiceContext);
                if (slots != null)
                {
                    Flash();

                    if (!slots.TryLoad(snapshot))
                        CardCmd.PreviewCardPileAdd(
                            await CardPileCmd.AddGeneratedCardToCombat(
                                snapshot.Card, PileType.Draw, player, CardPilePosition.Random));
                    else
                        MagicWineFoxSpellCmd.RefreshWandCardValues(slots);
                }
            }

            await PowerCmd.Remove(this);
        }
    }
}
