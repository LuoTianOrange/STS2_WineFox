using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.MagicWineFoxCard
{
    [RegisterCard(typeof(MagicWineFoxSpellModifierCardPool))]
    public class ChainReaction() : MagicWineFoxSpellCard(
        1, CardType.Skill, CardRarity.Rare, TargetType.None), IMagicWineFoxSpellModifierCard
    {
        private int _replayCount;

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardChainReaction);

        public override string SpellIconPath => Const.Paths.SpellIconChainReaction;

        public void ApplyModifier(MagicWineFoxSpellModifierState modifiers)
        {
            if (_replayCount <= 0)
                return;

            modifiers.AddExtraCasts(_replayCount);
            modifiers.MarkExtraCastsBudgetFree();
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            _replayCount = MagicWineFoxSpellCmd.GetPower(Owner)?.RemoveAllOverloads() ?? 0;
            await MagicWineFoxSpellCmd.Load(choiceContext, this, play, true);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
