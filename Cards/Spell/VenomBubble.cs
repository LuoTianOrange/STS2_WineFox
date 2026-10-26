using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class VenomBubble() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new PowerVar<PoisonPower>(3m),
        ];

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromPower<PoisonPower>(),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardVenomBubble);

        public override string SpellIconPath => Const.Paths.SpellIconVenomBubble;

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner;
            if (owner?.Creature == null)
                return;

            var targets = ResolveSpellTargets(context);
            if (targets.Count == 0)
                return;

            await PowerCmd.Apply<PoisonPower>(
                context.ChoiceContext,
                targets,
                DynamicVars.Poison.BaseValue,
                owner.Creature,
                context.SourceCard);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);

            if (Owner is not { } owner)
                return;

            await SpellModifierFactory.ChooseOneOfRandomToHand(
                choiceContext, owner, Rarity, exactRarity: Rarity);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Poison.UpgradeValueBy(2m);
        }
    }
}
