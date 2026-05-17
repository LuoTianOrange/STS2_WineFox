using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class Scratch() : WineFoxCard(
        0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        protected override bool HasEnergyCostX => true;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
            [
                ModCardVars.Computed(
                    "TotalDamage",
                    0m,
                    card => CalculateTotalDamage(card, CardPreviewMode.None, null, runGlobalHooks: true),
                    CalculateTotalDamagePreview)
            ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardScratch);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target");

            var x = ResolveEnergyXValue();
            var hits = 2 * x;
            if (hits <= 0) return;

            var damage = CalculateDamagePerHit(this, x);

            await DamageCmd.Attack(damage)
                .WithHitCount(hits)
                .FromCard(this)
                .Targeting(play.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
        }

        private static decimal CalculateTotalDamagePreview(
            CardModel? card,
            CardPreviewMode previewMode,
            Creature? target,
            bool runGlobalHooks)
        {
            return CalculateTotalDamage(card, previewMode, target, runGlobalHooks);
        }

        private static decimal CalculateTotalDamage(
            CardModel? card,
            CardPreviewMode previewMode,
            Creature? previewTarget,
            bool runGlobalHooks)
        {
            var player = card?.Owner;
            if (player == null)
                return 0m;

            var combatState = player.Creature.CombatState;
            if (combatState == null)
                return 0m;

            var x = player.PlayerCombatState?.Energy ?? 0;
            if (x <= 0)
                return 0m;

            var nonNullCard = card!;
            var modifiedX = Hook.ModifyXValue(combatState, nonNullCard, x);
            if (modifiedX <= 0)
                return 0m;

            var damagePerHit = CalculateDamagePerHit(nonNullCard, modifiedX);
            if (runGlobalHooks)
            {
                var target = previewTarget ?? combatState.HittableEnemies.FirstOrDefault();
                if (target != null)
                    damagePerHit = Hook.ModifyDamage(
                        nonNullCard.Owner.RunState,
                        combatState,
                        target,
                        player.Creature,
                        damagePerHit,
                        ValueProp.Move,
                        nonNullCard,
                        ModifyDamageHookType.All,
                        previewMode,
                        out _);
            }

            var hitCount = CalculateHitCount(modifiedX);
            return damagePerHit * hitCount;
        }

        private static decimal CalculateDamagePerHit(CardModel card, decimal x)
        {
            return card.IsUpgraded ? x + 2m : x;
        }

        private static decimal CalculateHitCount(decimal x)
        {
            return 2m * x;
        }
    }
}
