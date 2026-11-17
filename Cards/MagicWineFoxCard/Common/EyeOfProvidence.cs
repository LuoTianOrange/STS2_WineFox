using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EyeOfProvidence() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            [CardKeyword.Exhaust];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEyeOfProvidence);

        public override string SpellIconPath => Const.Paths.SpellIconEyeOfProvidence;

        public override SpellTraits Traits => new() { AttackSpell = false, TargetSelf = true, SupportSpell = true };

        public override int MaxUpgradeLevel => 2;

        private CardRarity ModifierRarity => CurrentUpgradeLevel switch
        {
            >= 2 => CardRarity.Rare,
            1 => CardRarity.Uncommon,
            _ => CardRarity.Common,
        };

        protected override void AddExtraArgsToDescription(LocString description)
        {
            base.AddExtraArgsToDescription(description);

            description.Add("Quality", new LocString("cards", CurrentUpgradeLevel switch
            {
                >= 2 => "STS2_WINE_FOX_CARD_EYE_OF_PROVIDENCE_QUALITY_2",
                1 => "STS2_WINE_FOX_CARD_EYE_OF_PROVIDENCE_QUALITY_1",
                _ => "STS2_WINE_FOX_CARD_EYE_OF_PROVIDENCE_QUALITY_0",
            }));
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            await Task.CompletedTask;
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);

            if (Owner is not { } owner)
                return;

            await CardPileCmd.Draw(choiceContext, 1, owner);
            await SpellModifierFactory.ChooseOneFromPoolToHand(
                choiceContext,
                owner,
                ModifierRarity,
                null,
                CurrentUpgradeLevel == 0 ? ModifierRarity : null);
        }
    }
}
