using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Powers;
using STS2_WineFox.Utils;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Combat.AttackHits;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Uncommon
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class DripstoneTrap() : WineFoxCard(
        2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy), IAttackHitHookListener
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(2m, ValueProp.Move),
            ModCardVars.Computed("Hits", 0m, CalcHits),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords => [WineFoxKeywords.StoneKeyword];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<VulnerablePower>()];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardDripstoneTrap);

        protected override bool IsPlayable
        {
            get
            {
                var creature = Owner?.Creature;
                if (creature == null) return false;

                var stone = creature.Powers.OfType<StonePower>().FirstOrDefault()?.Amount ?? 0;
                return stone > 0;
            }
        }

        public async Task BeforeAttackHit(AttackHitContext context)
        {
            if (!ReferenceEquals(context.CardSource, this) ||
                context.SingleTarget?.GetCreatureNode() is not { } nTarget)
                return;

            FallDripstone(nTarget.GlobalPosition);
            await VFXUtil.Wait(0.1f);
        }

        public async Task AfterAttackHit(AttackHitContext context)
        {
            if (!ReferenceEquals(context.CardSource, this))
                return;

            await PowerCmd.Apply<VulnerablePower>(
                new ThrowingPlayerChoiceContext(),
                context.Targets,
                1m,
                context.Dealer,
                this);
        }

        protected async void FallDripstone(Vector2 position)
        {
            VFXUtil.PlaySimple(Const.Paths.DripstoneVfx, position);
            await VFXUtil.Wait(0.06f);
            VFXUtil.PlaySFXSimple(Const.Audio.Dripstone);
            await VFXUtil.Wait(0.08f);
            VFXUtil.PlaySFXSimple(Const.Audio.Dripstone);
            await VFXUtil.Wait(0.09f);
            VFXUtil.PlaySFXSimple(Const.Audio.Dripstone);
        }

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner?.Creature;
            if (owner == null) return;
            if (owner.CombatState is not { } combatState) return;

            var stoneAmountDecimal = await MaterialCmd.ConsumeAllMaterialOfTypeForSeries<StonePower>(this, play);
            var totalHits = (int)stoneAmountDecimal;
            if (totalHits <= 0) return;

            await DamageCmd.Attack(DynamicVars["Damage"].BaseValue)
                .FromCard(this, play)
                .TargetingRandomOpponents(combatState)
                .WithHitCount(totalHits)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }

        private static decimal CalcHits(CardModel? card)
        {
            var creature = card?._owner?.Creature;
            if (creature == null) return 0m;

            var stone = creature.Powers.OfType<StonePower>().FirstOrDefault()?.Amount ?? 0;
            return stone;
        }
    }
}
