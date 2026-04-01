using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class MassProductionPower : WineFoxPower
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.MassProductionPowerIcon);

        public override async Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
        {
            if (!addedByPlayer) return;

            if (card.IsClone) return;

            if (card.Owner?.Creature != Owner) return;

            if (Amount <= 0m) return;

            var combatState = Owner.CombatState;
            if (combatState == null) return;

            var teammates = combatState
                .GetTeammatesOf(Owner)
                .Where(c => !ReferenceEquals(c, Owner) && c is { IsPlayer: true, IsAlive: true })
                .ToList();

            if (teammates.Count == 0) return;

            foreach (var teammate in teammates)
            {
                if (teammate.Player == null) continue;

                var clone = card.CreateClone();

                combatState.RemoveCard(clone);
                combatState.AddCard(clone, teammate.Player);

                var instance = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, true);

                if (LocalContext.IsMe(teammate))
                    CardCmd.PreviewCardPileAdd(instance);
            }

            await PowerCmd.Decrement(this);
        }
    }
}
