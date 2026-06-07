using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using STS2_WineFox.Character;
using STS2_WineFox.Utils;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Milk() : WineFoxCard(
        1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMilk);

        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var creature = owner.Creature;

            var castDelay = owner.Character?.CastAnimDelay ?? 0f;

            await CreatureCmd.TriggerAnim(creature, "Cast", castDelay);
            var node = NCombatRoom.Instance?.GetCreatureNode(creature);
            if (node != null)
            {
                //喝牛奶的位置
                var mousePos = node.VfxSpawnPosition + new Vector2(25f, -20f);
                VFXUtil.PlaySimple(Const.Paths.DrinkMilkVfx, mousePos, 2.5f);
                await VFXUtil.Wait(0.7f);
                VFXUtil.PlaySFXSimple(Const.Audio.Drink, .7f);
            }
            // VfxCmd.PlayOnCreatureCenter(creature, "vfx/vfx_flying_slash");
            
            await MilkCleanseHelper.Cleanse(creature, creature, this);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
