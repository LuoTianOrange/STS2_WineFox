using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class SlashBladeWoodPower : WineFoxPower
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [ new("Vigorous", 1m)];

        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.SlashBladeWoodPowerIcon);
        
        public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
        {
            if (card?.Owner == null)
                return;

            var drawerCreature = card.Owner.Creature;

            var combatState = drawerCreature.CombatState;
            if (fromHandDraw
                || drawerCreature != Owner
                || combatState == null
                || combatState.CurrentSide != drawerCreature.Side)
            {
                return;
            }

            Flash();
            int vigorToGain = Amount;

            if (vigorToGain <= 0)
                vigorToGain = (int)DynamicVars["Vigorous"].BaseValue;

            await PowerCmd.Apply<VigorPower>(Owner, vigorToGain, Owner, null);
        }
    }
}
