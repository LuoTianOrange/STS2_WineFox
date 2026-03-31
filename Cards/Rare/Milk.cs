using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
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
            VfxCmd.PlayOnCreatureCenter(creature, "vfx/vfx_flying_slash");

            var powersSnapshot = creature.Powers.ToList();

            foreach (var power in powersSnapshot.Where(power => power is not MaterialPower)
                         .Where(power => power.Type == PowerType.Debuff)) await PowerCmd.Remove(power);
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}
