using MegaCrit.Sts2.Core.Commands;
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
    public class ConvertManaToDamage() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellLookBehindModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Bonus", 6m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardConvertManaToDamage);

        public override string SpellIconPath => Const.Paths.SpellIconConvertManaToDamage;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            var energy = modifiers.CurrentX;

            if (energy > 0)
                modifiers.AddDamageBonus(DynamicVars["Bonus"].BaseValue * energy);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var energy = Owner.PlayerCombatState?.Energy ?? 0;

            if (energy > 0)
                await PlayerCmd.LoseEnergy(energy, Owner);

            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, true, energy);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Bonus"].UpgradeValueBy(2m);
        }
    }
}
