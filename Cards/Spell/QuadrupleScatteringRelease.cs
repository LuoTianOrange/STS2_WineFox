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
    public class QuadrupleScatteringRelease() : MagicWineFoxSpellCard(
        3, CardType.Skill, CardRarity.Rare, TargetType.None), IMagicWineFoxSpellLookBehindModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("ExtraCasts", 3m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardQuadrupleScatteringRelease);

        public override string SpellIconPath => Const.Paths.SpellIconQuadrupleScatteringRelease;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            CardKeyword.Ethereal,
            CardKeyword.Exhaust
        ];

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.AddExtraCasts((int)DynamicVars["ExtraCasts"].BaseValue);
            modifiers.MarkExtraCastsBudgetFree();
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
