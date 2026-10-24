using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxSpellModifierCardPool))]
    public class GravitationalFieldPossession() : MagicWineFoxSpellCard(
        2, CardType.Skill, CardRarity.Common, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardGravitationalFieldPossession);

        public override string SpellIconPath => Const.Paths.SpellIconGravitationalFieldPossession;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.MarkShareDamageOnHit();
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
