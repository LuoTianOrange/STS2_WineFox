using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxTokenCardPool))]
    public class BloodSacrifice() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellWandModifierCard
    {
        protected override bool HasEnergyCostX => true;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBloodSacrifice);

        public override string SpellIconPath => Const.Paths.SpellIconBloodSacrifice;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            var x = modifiers.CurrentX;
            if (x > 0)
                modifiers.AddDamageBonus(2m * x);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var x = ResolveEnergyXValue();

            await CreatureCmd.Damage(
                new ThrowingPlayerChoiceContext(),
                Owner.Creature,
                x + 1m,
                ValueProp.Unblockable | ValueProp.Unpowered,
                null,
                null);

            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true, xValue: x);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
