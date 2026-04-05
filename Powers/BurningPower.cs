using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Combat;
using STS2RitsuLib.Combat.HealthBars;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Powers
{
    public class BurningPower : WineFoxPower, IHealthBarForecastSource
    {
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Counter;
        public override PowerAssetProfile AssetProfile => Icons(Const.Paths.BurningIcon);

        /// <inheritdoc />
        /// <remarks>
        ///     Orange is used as <see cref="HealthBarForecastSegment.Color" /> (lethal label); <c>Colors.White</c> is the
        ///     lane overlay modulate so <see cref="BurnHealthBarForecastMaterials" />'s doom-bar shader gradient is not
        ///     multiplied again by orange (<see cref="HealthBarForecasts.FromRight(HealthBarForecastContext, Color, Color?)" />).
        /// </remarks>
        public IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
        {
            if (context.Creature != Owner)
                return [];

            var damage = CalculateDamageOnNextTrigger();
            if (damage <= 0)
                return [];

            var order = HealthBarForecastOrder.ForSideTurnStart(context.Creature, Owner.Side);
            return HealthBarForecasts
                .FromRight(context, new(1f, 0.478f, 0f), Colors.White)
                .Add(damage, order, BurnHealthBarForecastMaterials.ForecastMaterial)
                .Build();
        }

        public int CalculateDamageOnNextTrigger()
        {
            var owner = Owner;
            var combatState = owner.CombatState ??
                              throw new InvalidOperationException("BurningPower owner is not in combat.");
            var damage = Hook.ModifyDamage(
                combatState.RunState,
                combatState,
                owner,
                null,
                Amount,
                ValueProp.Unblockable | ValueProp.Unpowered,
                null,
                ModifyDamageHookType.All,
                CardPreviewMode.None,
                out _);

            return (int)damage;
        }

        public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
        {
            if (side != Owner.Side) return;

            Flash();

            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                Owner,
                Amount,
                ValueProp.Unblockable | ValueProp.Unpowered,
                null,
                null);

            var newAmount = Math.Floor(Amount / 2m);
            var reduction = Amount - newAmount;
            if (reduction > 0m) await PowerCmd.ModifyAmount(this, -reduction, null, null);

            if (newAmount <= 0m)
                await PowerCmd.Remove(this);
        }
    }
}
