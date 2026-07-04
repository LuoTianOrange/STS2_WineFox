using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Relics.Event
{
    [RegisterRelic(typeof(WineFoxRelicPool))]
    public sealed class SilverCercisCrown : WineFoxRelic
    {
        public override RelicRarity Rarity => RelicRarity.Event;
        public override RelicAssetProfile AssetProfile => Icons(Const.Paths.SilverCercisCrownRelicIcon);

        public override async Task AfterDamageReceived(
            PlayerChoiceContext choiceContext,
            Creature target,
            DamageResult result,
            ValueProp props,
            Creature? dealer,
            CardModel? cardSource)
        {
            if (target != Owner.Creature || dealer == null || dealer.IsPlayer || result.TotalDamage <= 0)
                return;

            if (!props.IsPoweredAttack())
                return;

            Flash();
            await CreatureCmd.Damage(
                choiceContext,
                dealer,
                result.TotalDamage,
                ValueProp.Unblockable | ValueProp.Unpowered,
                Owner.Creature,
                null);
        }
    }
}
