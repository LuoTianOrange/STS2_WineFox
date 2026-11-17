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

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxSpellModifierCardPool))]
    public class BloodSacrifice() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new("Percent", 3m),
        ];

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            .. base.CanonicalKeywords,
            CardKeyword.Exhaust,
            CardKeyword.Ethereal,
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardBloodSacrifice);

        public override string SpellIconPath => Const.Paths.SpellIconBloodSacrifice;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            if (Owner?.Creature is not { } creature)
                return;

            var missing = Math.Max(0m, creature.MaxHp - creature.CurrentHp);
            if (missing <= 0m)
                return;

            modifiers.MultiplyDamage(1m + missing * DynamicVars["Percent"].BaseValue / 100m);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner?.Creature is not { } ownerCreature)
                return;

            await CreatureCmd.Damage(
                choiceContext,
                ownerCreature,
                1m,
                ValueProp.Unblockable | ValueProp.Unpowered,
                null,
                null);

            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars["Percent"].UpgradeValueBy(1m);
        }
    }
}
