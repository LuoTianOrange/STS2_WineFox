using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Combat.Magic;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class RechargeShield() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Common, TargetType.Self), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new BlockVar(9m, ValueProp.Move)
        ];

        public override bool GainsBlock => true;

        public override SpellTraits Traits => new() { AttackSpell = false, TargetSelf = true };

        public override decimal PreviewBlock => MagicBlock.Resolve(this, DynamicVars.Block.BaseValue);

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardRechargeShield);

        public override string SpellIconPath => Const.Paths.SpellIconRechargeShield;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner?.Creature is not { } owner)
                return;

            await CreatureCmd.GainBlock(
                owner,
                context.BlockWithModifiers(DynamicVars.Block.BaseValue),
                ValueProp.Unpowered,
                null);

            if (!context.IsAffectedByModifiers)
                return;

            var power = await PowerCmd.Apply<Powers.RechargeSpellPower>(
                context.ChoiceContext,
                owner,
                1m,
                owner,
                context.CastSourceCard);

            power?.Bind(new MagicWineFoxSpellSlotSnapshot(
                context.SourceCard.CreateClone(),
                null,
                false,
                0,
                context.SourceCard));
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Block.UpgradeValueBy(2m);
        }
    }
}
