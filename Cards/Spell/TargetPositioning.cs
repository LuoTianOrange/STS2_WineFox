using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxSpellModifierCardPool))]
    public class TargetPositioning() : MagicWineFoxSpellCard(
        0, CardType.Skill, CardRarity.Token, TargetType.AnyEnemy), IMagicWineFoxSpellWandModifierCard
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardTargetPositioning);

        public override string SpellIconPath => Const.Paths.SpellIconTargetPositioning;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.MarkForcedTarget(modifiers.CurrentTarget);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);

            if (!IsUpgraded ||
                play.Target is not { } target ||
                Owner?.Creature is not { } creature)
                return;

            await PowerCmd.Apply<TargetPositioningPower>(
                choiceContext,
                target,
                1m,
                creature,
                this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}
