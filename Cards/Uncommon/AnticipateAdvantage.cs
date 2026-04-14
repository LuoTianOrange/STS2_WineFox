using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Powers;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    public class AnticipateAdvantage() : WineFoxCard(
        1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
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
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var target = play.Target;
            if (target.Monster?.IntendsToAttack != true) return;

            var combatState = Owner.Creature.CombatState;
            if (combatState == null) return;

            var enemyCount = combatState.HittableEnemies.Count();
            if (enemyCount <= 0) return;

            var dexAmount = DynamicVars["Dex"].BaseValue * enemyCount;
            await PowerCmd.Apply<AnticipateAdvantageDexPower>(Owner.Creature, dexAmount, Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            AddKeyword(CardKeyword.Innate);
        }
    }
}
