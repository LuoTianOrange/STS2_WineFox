using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
            new("Burn", 4m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSearingJolt);

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

            var amount = DynamicVars["Burn"].BaseValue;

            if (target.HasPower<BurningPower>())
                amount += 2m;

            await PowerCmd.Apply<BurningPower>(
                new ThrowingPlayerChoiceContext(),
                target,
                amount,
                owner.Creature,
                context.SourceCard);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Burn"].UpgradeValueBy(2);
        }
    }
}
