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
    public class CombinedArms() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellLookBehindModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("LookBehindDamage", 4m),
            new("NextBlock", 6m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardCombinedArms);

        public override string SpellIconPath => Const.Paths.SpellIconCombinedArms;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.AddDamageBonus(DynamicVars["LookBehindDamage"].BaseValue);
            modifiers.MarkNextBlockBonus(DynamicVars["NextBlock"].BaseValue);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["LookBehindDamage"].UpgradeValueBy(2m);
            DynamicVars["NextBlock"].UpgradeValueBy(2m);
        }
    }
}
