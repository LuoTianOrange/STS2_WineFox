using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxTokenCardPool))]
    public class SpellOverload() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CardsVar(1)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSpellOverload);

        public override string SpellIconPath => Const.Paths.SpellIconSpellOverload;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            WineFoxKeywords.LoadKeyword,
            CardKeyword.Ethereal,
            CardKeyword.Exhaust
        ];

        public override SpellTraits Traits =>
            new() { AttackSpell = false, SupportSpell = true, TargetSelf = true };

        public int OverloadCount { get; set; }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            if (context.Owner is not { } owner || owner.Creature is not { } caster)
                return;

            var choiceContext = context.ChoiceContext;

            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, owner);

            if (IsUpgraded)
                await PlayerCmd.GainEnergy(1m, owner);

            var amount = Math.Max(1, OverloadCount);

            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                caster,
                amount,
                ValueProp.Unblockable | ValueProp.Unpowered,
                context.SourceCard,
                null);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
