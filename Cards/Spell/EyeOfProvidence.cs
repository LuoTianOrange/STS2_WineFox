using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class EyeOfProvidence() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellCard
    {
        public override IEnumerable<CardKeyword> CanonicalKeywords =>
            base.CanonicalKeywords.Append(CardKeyword.Exhaust);

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEyeOfProvidence);

        public override string SpellIconPath => Const.Paths.SpellIconEyeOfProvidence;
        
        public override bool TargetsEnemy => false;

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play);
        }

        public async Task CastAsSpell(MagicWineFoxSpellCastContext context)
        {
            var owner = context.Owner;
            if (owner?.Creature?.CombatState == null)
                return;

            await SpellModifierFactory.ChooseOneToHand(context.ChoiceContext, owner);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
