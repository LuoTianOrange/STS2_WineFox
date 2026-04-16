using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class HighlyFocused() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars => [];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardHighlyFocused);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var creature = Owner.Creature;
            var strength = creature.GetPowerAmount<StrengthPower>();

            await PowerCmd.Apply<HighlyFocusedTrackerPower>(creature, 1m, creature, this);

            if (strength > 0m)
            {
                var tracker = creature.Powers.OfType<HighlyFocusedTrackerPower>().FirstOrDefault();
                if (tracker != null)
                    await tracker.ApplyInitialBonus(strength);
            }
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
