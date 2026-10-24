using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxSpellModifierCardPool))]
    public class HeavyScattering() : MagicWineFoxSpellCard(
        2, CardType.Skill, CardRarity.Rare, TargetType.None), IMagicWineFoxSpellWandModifierCard
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardHeavyScattering);

        public override string SpellIconPath => Const.Paths.SpellIconHeavyScattering;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
            CardKeyword.Exhaust
        ];

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.AddExtraRounds(1);
            modifiers.MarkRandomTargets();
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, true);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
