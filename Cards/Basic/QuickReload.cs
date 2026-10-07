using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Basic
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    [RegisterCharacterStarterCard(typeof(MagicWineFox), 1)]
    public class QuickReload() : WineFoxCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardQuickReload);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner is not { } owner)
                return;

            var spells = PileType.Draw.GetPile(owner).Cards
                .Where(card => card is Spell.MagicWineFoxSpellCard)
                .ToList();

            if (spells.Count > 0)
            {
                var index = owner.RunState.Rng.CombatCardGeneration.NextInt(spells.Count);
                await CardPileCmd.Add(spells[index], PileType.Hand, CardPilePosition.Top, null, false);
            }

            await MagicWineFoxSpellCmd.GiveDoubleReleaseSigil(owner);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
