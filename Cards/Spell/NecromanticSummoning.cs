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
    public class NecromanticSummoning() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Token, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new SummonVar(20m)
        ];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardNecromanticSummoning);

        public override string SpellIconPath => Const.Paths.SpellIconNecromanticSummoning;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            modifiers.MarkSummonOnKill(DynamicVars.Summon.BaseValue);
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, isModifier: true);
        }

        protected override void OnUpgrade()
        {
            DynamicVars.Summon.UpgradeValueBy(3m);
        }
    }
}
