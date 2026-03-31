using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization;
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
                .Where(c => !ReferenceEquals(c, Owner) && c.IsPlayer && c.IsAlive)
                .ToList();

            if (teammates.Count == 0)
            {
                return;
            }

            foreach (var teammate in teammates)
            {
                var clone = card.CreateClone();


                var instance = await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, true);

                CardCmd.PreviewCardPileAdd(instance);
            }

            await PowerCmd.ModifyAmount(this, -1m, null, card);

            if (Amount <= 0m)
                await PowerCmd.Remove(this);
        }
    }
}
