using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxSpellModifierCardPool))]
    public class Catalyst() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Rare, TargetType.None), IMagicWineFoxSpellLookBehindModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Amplify", 1m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCatalyst);

        public override string SpellIconPath => Const.Paths.SpellIconCatalyst;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
            CardKeyword.Exhaust
        ];

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.MultiplyDebuffs(1m + DynamicVars["Amplify"].BaseValue);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Amplify"].UpgradeValueBy(1m);
        }
    }
}
