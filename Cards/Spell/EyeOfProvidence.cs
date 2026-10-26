using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
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
            await SpellModifierFactory.ChooseOneToHand(
                choiceContext, owner, exactRarity: Rarity);
        }

        protected override void OnUpgrade()
        {
            RemoveKeyword(CardKeyword.Exhaust);
        }
    }
}
