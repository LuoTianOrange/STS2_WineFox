using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models.Capabilities;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Common
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class SpellPop() : WineFoxCard(
        1, CardType.Skill, CardRarity.Common, TargetType.None)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardSpellPop);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            if (Owner is not { } owner)
                return;

            var power = MagicWineFoxSpellCmd.GetPower(owner);
            if (power?.DrainOldest() is not { } oldest)
                return;

            await CardPileCmd.Add(oldest.Card, PileType.Hand);
            oldest.Card.GetOrCreateCapability<TemporaryRetainCapability>();
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
