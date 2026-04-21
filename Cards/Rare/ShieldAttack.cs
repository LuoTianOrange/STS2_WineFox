using System.Linq;
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
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class ShieldAttack() : WineFoxCard(
        1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        private static readonly AttachedState<CardModel, ShieldAttackSeriesState> SeriesStates =
            new(() => new());

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new IntVar("BlockMultiplier", 2m),
            new CardsVar(1),
            ModCardVars.Computed("TotalDamage", 0m, CalcTotalDamage, CalcTotalDamagePreview),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardShieldAttack);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner;

            var creature = owner.Creature;

            var combatState = creature.CombatState;
            if (combatState is null) return;

            var state = SeriesStates.GetOrCreate(this);
            var damage = state.Damage;
            var blockToLose = 0m;

            if (play.IsFirstInSeries)
            {
                var block = creature.Block;
                damage = block * DynamicVars["BlockMultiplier"].BaseValue;
                state.Damage = damage;
                blockToLose = block;
            }

            await DamageCmd.Attack(damage)
                .FromCard(this)
                .TargetingAllOpponents(combatState)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            if (blockToLose > 0m) await CreatureCmd.LoseBlock(creature, blockToLose);

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }

        private static decimal CalcTotalDamage(CardModel? card) =>
            ResolveTotalDamageAfterModifiers(card, CardPreviewMode.None, null, runGlobalHooks: true);

        private static decimal CalcTotalDamagePreview(
            CardModel? card,
            CardPreviewMode previewMode,
            Creature? target,
            bool runGlobalHooks) =>
            ResolveTotalDamageAfterModifiers(card, previewMode, target, runGlobalHooks);

        private static decimal ResolveTotalDamageAfterModifiers(
            CardModel? card,
            CardPreviewMode previewMode,
            Creature? previewTarget,
            bool runGlobalHooks)
        {
            if (card == null) return 0m;
            if (!card.DynamicVars.TryGetValue("BlockMultiplier", out var multVar)) return 0m;
            var dealer = card._owner?.Creature;
            if (dealer == null) return 0m;

            var baseDamage = dealer.Block * multVar.BaseValue;
            var combatState = dealer.CombatState;
            if (combatState == null || !runGlobalHooks)
                return baseDamage;

            var target = previewTarget;
            if (target == null && card.TargetType == TargetType.AllEnemies)
                target = combatState.HittableEnemies.FirstOrDefault();

            if (target == null)
                return baseDamage;

            return Hook.ModifyDamage(
                card.Owner.RunState,
                combatState,
                target,
                dealer,
                baseDamage,
                ValueProp.Move,
                card,
                ModifyDamageHookType.All,
                previewMode,
                out _);
        }

        private sealed class ShieldAttackSeriesState
        {
            public decimal Damage { get; set; }
        }
    }
}
