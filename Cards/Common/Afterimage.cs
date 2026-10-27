using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Cards.Spell;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(MagicWineFoxSpellModifierCardPool))]
    public class Afterimage() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
            CardKeyword.Exhaust,
        ];

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Copies", 1m),
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardAfterimage);

        public override string SpellIconPath => Const.Paths.SpellIconAfterimage;

        public override SpellTraits Traits => new() { AttackSpell = false, SupportSpell = true };

        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
        [
            HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        ];

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.AddCopyAfterCast((int)DynamicVars["Copies"].BaseValue);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Copies"].UpgradeValueBy(1m);
        }
    }
}
