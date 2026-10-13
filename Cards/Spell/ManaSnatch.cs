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
    public class ManaSnatch() : MagicWineFoxSpellCard(
        2, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Ratio", 50m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardManaSnatch);

        public override string SpellIconPath => Const.Paths.SpellIconManaSnatch;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.MarkDamageToBlock(DynamicVars["Ratio"].BaseValue / 100m);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Ratio"].UpgradeValueBy(50m);
        }
    }
}
