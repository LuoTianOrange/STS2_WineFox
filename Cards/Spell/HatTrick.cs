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
    public class HatTrick() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellWandModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("ExtraCasts", 2m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardHatTrick);

        public override string SpellIconPath => Const.Paths.SpellIconHatTrick;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            if (modifiers.WandSpellCount != 1)
                return;

            modifiers.AddExtraCasts((int)DynamicVars["ExtraCasts"].BaseValue);
            modifiers.MarkExtraCastsBudgetFree();
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
