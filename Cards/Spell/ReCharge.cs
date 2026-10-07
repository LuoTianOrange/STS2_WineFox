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
    public class ReCharge() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellWandModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CardsVar(1)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardReCharge);

        public override string SpellIconPath => Const.Paths.SpellIconReCharge;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.MarkDrawPerSpell(DynamicVars.Cards.BaseValue, IsUpgraded);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);
        }

        protected override void OnUpgrade()
        { }
    }
}
