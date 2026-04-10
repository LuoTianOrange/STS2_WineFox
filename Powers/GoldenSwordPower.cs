using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Cards.Token;
using STS2_WineFox.Cards.Token.HellGift;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class GoldenSwordPower : WineFoxPower
    {
        private bool _noEthereal;
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override bool IsInstanced => true;


        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.GoldenSwordPowerIcon);

        protected override IEnumerable<DynamicVar> CanonicalVars => [new("AppliesEthereal", 1m), new EnergyVar(1)];

        public override int DisplayAmount
        {
            get
            {
                var d = GetInternalData<Data>();
                return 2 - d.AttacksPlayed % 2;
            }
        }

        protected override object InitInternalData()
        {
            return new Data();
        }

        public override Task AfterApplied(Creature? applier, CardModel? cardSource)
        {
            _noEthereal = cardSource is GoldenSword { IsUpgraded: true };
            DynamicVars["AppliesEthereal"].BaseValue = _noEthereal ? 0m : 1m;
            if (!_noEthereal)
                ApplyEtherealToHand();
            InvokeDisplayAmountChanged();
            return Task.CompletedTask;
        }

        private void ApplyEtherealToHand()
        {
            var player = Owner.Player;
            if (player == null) return;

            foreach (var card in PileType.Hand.GetPile(player).Cards)
                if (card.Type == CardType.Attack)
                    card.AddKeyword(CardKeyword.Ethereal);
        }

        public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (_noEthereal) return Task.CompletedTask;
            if (card?.Owner?.Creature != Owner) return Task.CompletedTask;
            if (card.Type != CardType.Attack) return Task.CompletedTask;

            card.AddKeyword(CardKeyword.Ethereal);
            return Task.CompletedTask;
        }

        public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
        {
            if (cardPlay.IsAutoPlay) return;
            if (cardPlay.Card?.Owner?.Creature != Owner) return;
            if (cardPlay.Card.Type != CardType.Attack) return;

            var d = GetInternalData<Data>();
            d.AttacksPlayed++;
            InvokeDisplayAmountChanged();

            var triggers = d.AttacksPlayed / 2 - d.TriggerCount;
            if (triggers <= 0) return;

            Flash();
            var player = Owner.Player;
            if (player != null)
                for (var i = 0; i < triggers; i++)
                    await PlayerCmd.GainEnergy(1, player);

            d.TriggerCount += triggers;
            InvokeDisplayAmountChanged();
        }

        public override Task BeforeSideTurnStart(
            PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
        {
            if (side != Owner.Side) return Task.CompletedTask;

            var d = GetInternalData<Data>();
            d.AttacksPlayed = 0;
            d.TriggerCount = 0;
            InvokeDisplayAmountChanged();
            return Task.CompletedTask;
        }

        public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
        {
            if (_noEthereal) return Task.CompletedTask;
            if (card?.Owner?.Creature != Owner) return Task.CompletedTask;
            if (card.Type != CardType.Attack) return Task.CompletedTask;

            card.AddKeyword(CardKeyword.Ethereal);
            return Task.CompletedTask;
        }

        private class Data
        {
            public int AttacksPlayed;
            public int TriggerCount;
        }
    }
}
