using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2_WineFox.Character;
using STS2_WineFox.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace STS2_WineFox.Cards.Rare
{
    [RegisterCard(typeof(WineFoxCardPool))]
    public class EternalMelody() : WineFoxCard(
        0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
            [HoverTipFactory.FromPower<ChantPower>()];

        public override CardAssetProfile AssetProfile => Art(Const.Paths.CardEternalMelody);

        protected override async Task OnPlay(
            PlayerChoiceContext choiceContext,
            CardPlay play)
        {
            var owner = Owner.Creature;

            await PowerCmd.Apply<EternalMelodyRetentionPower>(choiceContext, owner, 1m, owner, this);

            if (IsUpgraded)
                await PowerCmd.Apply<ChantPower>(choiceContext, owner, 1m, owner, this);
        }

        protected override void OnUpgrade()
        {
        }
    }
}





