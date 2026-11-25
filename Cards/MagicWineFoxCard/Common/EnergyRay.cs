using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EnergyRay() : MagicWineFoxSpellCard(
        0, CardType.Attack, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        private const decimal ChargeAmount = 3m;

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(8m, ValueProp.Move),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEnergyRay);

        public override string SpellIconPath => Const.Paths.SpellIconEnergyRay;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<ChargePower>(),
        ];

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner;
            if (owner?.Creature == null)
                return;

            if (owner.Creature.CombatState is not { } combatState)
                return;

            var targets = ResolveSpellTargets(context);
            if (targets.Count == 0)
                return;

            var attack = DamageCmd.Attack(context.DamageWithModifiers(DynamicVars.Damage.BaseValue))
                .FromCard(context.SourceCard, context.Play)
                .WithAttackerAnim("Cast", 0.5f)
                .BeforeDamage(async () =>
                {
                    var beamTarget = targets[^1];
                    var beam = NHyperbeamVfx.Create(owner.Creature, beamTarget);

                    if (beam != null)
                    {
                        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(beam);
                        await Cmd.Wait(0.5f);
                    }

                    foreach (var enemy in targets)
                    {
                        var impact = NHyperbeamImpactVfx.Create(owner.Creature, enemy);

                        if (impact != null)
                            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(impact);
                    }
                });

            if (context.TargetsAllEnemies)
                await attack.TargetingAllOpponents(combatState).Execute(context.ChoiceContext);
            else
                await attack.Targeting(targets[0]).Execute(context.ChoiceContext);

            await DealExtraDamageStrikes(context, DynamicVars.Damage.BaseValue);

            await ChargePower.Gain(context.ChoiceContext, owner.Creature, ChargeAmount, context.SourceCard);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3m);
        }
    }
}
