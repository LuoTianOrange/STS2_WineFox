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
    public class AnticipateAdvantage() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [new IntVar("Dex", 3m)];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAnticipateAdvantage);

        protected override bool ShouldGlowGoldInternal =>
            CombatState != null &&
            CombatState.HittableEnemies.Any(e => e.Monster?.IntendsToAttack == true);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var combatState = Owner.Creature.CombatState;
            if (combatState == null) return;

            if (!combatState.HittableEnemies.Any(e => e.Monster?.IntendsToAttack == true)) return;

            await PowerCmd.Apply<DexterityPower>(Owner.Creature, DynamicVars["Dex"].BaseValue,
                Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Dex"].UpgradeValueBy(1m); // 3 → 4
        }
    }
}
