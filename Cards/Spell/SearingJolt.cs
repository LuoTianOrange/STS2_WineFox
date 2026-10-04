using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class SearingJolt() : MagicWineFoxSpellCard(
        1, CardType.Attack, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Burn", 4m),
            new HpLossVar(1m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardMagicOverloaded);

        public override string SpellIconPath => Const.Paths.SpellIconSearingJolt;
        
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<BurningPower>()
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }
        
        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner;
            var target = ResolveSpellTarget(context);
            if (owner?.Creature == null || target == null)
                return;

            await PowerCmd.Apply<BurningPower>(
                new ThrowingPlayerChoiceContext(),
                target,
                DynamicVars["Burn"].BaseValue,
                owner.Creature,
                context.SourceCard);

            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                owner.Creature,
                DynamicVars.HpLoss.BaseValue,
                ValueProp.Unpowered,
                null,
                null);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Burn"].BaseValue += 2m;
        }
    }
}
