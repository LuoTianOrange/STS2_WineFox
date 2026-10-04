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
    [RegisterCard(typeof(MagicWineFoxTokenCardPool))]
    public class QuadrupleScatteringRelease() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellLookBehindModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("ExtraCasts", 3m),
            new("DamageReduction", 75m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardQuadrupleScatteringRelease);

        public override string SpellIconPath => Const.Paths.SpellIconQuadrupleScatteringRelease;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            var casts = (int)DynamicVars["ExtraCasts"].BaseValue;
            var reduction = DynamicVars["DamageReduction"].BaseValue;

            modifiers.AddExtraCasts(casts);
            modifiers.MultiplyDamage(1m - reduction / 100m);
            modifiers.MarkRandomTargets();
            modifiers.MarkExtraCastsBudgetFree();
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);
        }

        protected override void OnUpgrade()
        { }
    }
}
