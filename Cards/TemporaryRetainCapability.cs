using System.Text.Json.Nodes;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models.Capabilities;

namespace STS2_WineFox.Cards
{
    [RegisterModelCapability(StableEntryStem = "temporary_retain")]
    public sealed class TemporaryRetainCapability : CardCapability
    {
        private const string AddedRetainKey = "addedRetain";
        private bool _addedRetain;

        protected override void OnAttach(CardModel owner)
        {
            if (owner.Keywords.Contains(CardKeyword.Retain))
                return;

            owner.AddKeyword(CardKeyword.Retain);
            _addedRetain = true;
        }

        protected override void OnDetach(CardModel owner)
        {
            if (_addedRetain && owner.Keywords.Contains(CardKeyword.Retain))
                owner.RemoveKeyword(CardKeyword.Retain);
        }

        protected override JsonNode SaveAdditionalState()
        {
            return new JsonObject
            {
                [AddedRetainKey] = _addedRetain,
            };
        }

        protected override void LoadAdditionalState(JsonNode? state, int schemaVersion)
        {
            _addedRetain = state?[AddedRetainKey]?.GetValue<bool>() ?? false;
        }

        public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (Owner?.Owner != player)
                return Task.CompletedTask;

            RemoveFromOwner();
            return Task.CompletedTask;
        }
    }
}
