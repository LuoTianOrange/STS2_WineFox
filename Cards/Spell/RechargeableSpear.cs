using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class RechargeableSpear() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new DamageVar(12m, ValueProp.Move)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRechargeableSpear);

        public override string SpellIconPath => Const.Paths.SpellIconRechargeableSpear;

        public override decimal PreviewDamage => DynamicVars.Damage.BaseValue;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            await DealSpellDamage(context, DynamicVars.Damage.BaseValue);

            if (!context.IsAffectedByModifiers)
                return;

            if (context.Owner?.Creature is not { } ownerCreature)
                return;

            var power = await PowerCmd.Apply<Powers.RechargeSpellPower>(
                context.ChoiceContext,
                ownerCreature,
                1m,
                ownerCreature,
                context.CastSourceCard);

            power?.Bind(new MagicWineFoxSpellSlotSnapshot(
                context.SourceCard.CreateClone(),
                context.Target,
                false,
                0,
                context.SourceCard));
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Damage.UpgradeValueBy(3m);
        }
    }
}
