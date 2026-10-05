using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EnergyRay() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(14m, ValueProp.Move)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicMissile);

        public override string SpellIconPath => Const.Paths.SpellIconEnergyRay;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromCard<Debris>(IsUpgraded),
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

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
                .FromCard(context.SourceCard, null)
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

            var debris = combatState.CreateCard<Debris>(owner);
            var instance = await CardPileCmd.AddGeneratedCardToCombat(debris, PileType.Hand, owner);
            CardCmd.PreviewCardPileAdd(instance);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(2m);
        }
    }
}
