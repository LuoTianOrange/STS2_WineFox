using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
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
            [new IntVar("Dex", 2m)];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<DexterityPower>()];
        
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

            var attackingEnemyCount = combatState.HittableEnemies.Count(e => e.Monster?.IntendsToAttack == true);
            if (attackingEnemyCount <= 0) return;

            var dexGain = IsUpgraded
                ? DynamicVars["Dex"].BaseValue * attackingEnemyCount
                : DynamicVars["Dex"].BaseValue;

            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, dexGain,
                Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
