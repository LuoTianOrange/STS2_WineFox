using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2_WineFox.Character;
using STS2_WineFox.Commands;
using STS2_WineFox.Mechanics;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(MagicWineFoxCardPool))]
    public class Reproduction() : WineFoxCard(
        1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardReproduction);

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
        {
            var owner = Owner;
            if (owner?.Creature == null)
                return;
            
            var slots = MagicWineFoxSpellCmd.GetPower(owner);

            // 只认法术本体：法术修正符不算「第一个法术」。
            var first = slots?.Slots.FirstOrDefault(
                snapshot => snapshot != null && ReproductionPower.IsBindable(snapshot.Card));

            var power = await PowerCmd.Apply<ReproductionPower>(
                choiceContext,
                owner.Creature,
                1m,
                owner.Creature,
                this);

            power?.Bind(first);
        }

        protected override void OnUpgrade()
        {
            EnergyCost.UpgradeBy(-1);
        }
    }
}
