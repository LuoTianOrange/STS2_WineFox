using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Spell
{
    [RegisterCard(typeof(MagicWineFoxTokenCardPool))]
    public class DoubleReleaseSigil() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardDoubleReleaseSigil);

        public override string SpellIconPath => Const.Paths.SpellIconDoubleReleaseSigil;

        public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [
            WineFoxKeywords.LoadKeyword,
            WineFoxKeywords.SpellModifierKeyword,
            CardKeyword.Ethereal,
            CardKeyword.Exhaust
        ];

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.AddExtraCasts(1);
            modifiers.MarkExtraCastsBudgetFree();
        }
        
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);
        }

        protected override void OnUpgrade()
        {
            // 衍生牌不可升级。
        }
    }
}
